using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DatabaseModel;
using Microsoft.EntityFrameworkCore;

//TODO: check exceptions thrown and activate validation methods
//TODO: check using singleordefault vs firstordefault vs any
//TODO: (optionally?) add .asnotracking clause to reads for performance for read only actions

namespace WebApplication_MVC.Models
{
    public class FieldOfInterestViewRepository : ICrudRepositoryAsync<FieldOfInterestView, string>
    {
        private DatabaseContext _context;

        public FieldOfInterestViewRepository(DatabaseContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistAsync(string id)
        {
            return await _context.FieldsOfInterest.AnyAsync(x => x.FieldOfInterestId == id);
        }

        public async Task<FieldOfInterestView> ReadAsync(string id)
        {
            FieldOfInterest field = await _context.FieldsOfInterest.Include(x => x.Descriptions)
                                                  .FirstOrDefaultAsync(x => x.FieldOfInterestId == id);
            if(field == null)
            {
                return null;
            }

            return MapToFieldOfInterestView(field);
        }

        public async Task<IEnumerable<FieldOfInterestView>> ReadAllAsync()
        {
            List<FieldOfInterestView> viewFields = new List<FieldOfInterestView>();

            await _context.FieldsOfInterest.Include(x => x.Descriptions)
                          .AsNoTracking()
                          .ForEachAsync(field => viewFields.Add(MapToFieldOfInterestView(field)));                         
            
            return viewFields;
        }

        private FieldOfInterestView MapToFieldOfInterestView(FieldOfInterest field)
        {
            string languageId;
            string description = DescriptionHelper.GetDescription(field.Descriptions, out languageId);

            return new FieldOfInterestView
                        {
                            FieldOfInterestId = field.FieldOfInterestId,
                            LanguageId = languageId,
                            Description = description,
                            Created = field.Created,
                            CreatedBy = field.CreatedBy
                        };
        }

        private FieldOfInterest MapToFieldOfInterest(FieldOfInterestView field)
        {
            return new FieldOfInterest
                        {
                            FieldOfInterestId = field.FieldOfInterestId,
                            Created = field.Created,
                            CreatedBy = field.CreatedBy
                        };
        }

        private FieldOfInterestDescription MapToFieldOfInterestDescription(FieldOfInterestView field)
        {
            return new FieldOfInterestDescription
                        {
                            FieldOfInterestId = field.FieldOfInterestId,
                            LanguageId = field.LanguageId,
                            Description = field.Description
                        };
        }

        public bool CreateValid(FieldOfInterestView field) //, bool checkExist = true)
        {
            //if (String.IsNullOrEmpty(field.FieldOfInterestId))
            //{
            //    ModelState.AddModelError(nameof(field.FieldOfInterestId), "Field of interest cannot be empty");
            //}
            //else if (field.FieldOfInterestId.Length > 2)
            //{
            //    ModelState.AddModelError(nameof(field.FieldOfInterestId), $"Field of interest {field.FieldOfInterestId} exceeds length of 2 chracters");
            //}
            //else if (checkExist && _context.FieldsOfInterest.Find(field.FieldOfInterestId) != null)
            //{
            //    ModelState.AddModelError(nameof(field.FieldOfInterestId), $"Field of interterest {field.FieldOfInterestId} already exists");
            //}

            //if (field.Description != null && field.Description.Length > 80)
            //{
            //    ModelState.AddModelError(nameof(field.Description), "Field of interest description exceeds length of 80 characters");
            //}

            return true;

            // + should not exist yet
        }
        public async Task<FieldOfInterestView> CreateAsync(FieldOfInterestView field)
        {
            if (!CreateValid(field))
            {
                throw new InvalidOperationException();
            }
            await _context.FieldsOfInterest.AddAsync(MapToFieldOfInterest(field));
            if (!String.IsNullOrEmpty(field.Description))
            {
                await _context.FieldOfInterestDescriptions.AddAsync(MapToFieldOfInterestDescription(field));
            }
            await _context.SaveChangesAsync();
            return field;
        }

        public bool UpdateValid(FieldOfInterestView field)
        {
            return CreateValid(field); // , false);
            // + should still exist 
        }
        public async Task<FieldOfInterestView> UpdateAsync(FieldOfInterestView field)
        {
            if (!UpdateValid(field))
            {
                throw new InvalidOperationException();
            }

            var fieldOfInterest = await _context.FieldsOfInterest.FirstOrDefaultAsync(x => (x.FieldOfInterestId == field.FieldOfInterestId));
            if (fieldOfInterest == null)
            {
                // TODO: throw a better exception
                throw new InvalidOperationException();
                //throw new DbUpdateConcurrencyException($"Field of interest {field.FieldOfInterestId} no longer exists", ...);
            }           
            //fieldOfInterest.xxx = field.xxx;

            var fieldOfInterestDescription = await _context.FieldOfInterestDescriptions.FirstOrDefaultAsync(x => (x.FieldOfInterestId == field.FieldOfInterestId && x.LanguageId == field.LanguageId));
            if (fieldOfInterestDescription == null)
            {
                if (!String.IsNullOrEmpty(field.Description))
                {
                    await _context.FieldOfInterestDescriptions.AddAsync(MapToFieldOfInterestDescription(field));
                }
            }
            else
            {
                fieldOfInterestDescription.Description = field.Description;

                if (!String.IsNullOrEmpty(field.Description))
                {
                    _context.FieldOfInterestDescriptions.Update(fieldOfInterestDescription);
                }
                else
                {
                    _context.FieldOfInterestDescriptions.Remove(fieldOfInterestDescription);
                }
            }
            await _context.SaveChangesAsync();
            return field;
        }

        public bool DeleteValid(FieldOfInterestView field)
        {
            return true;
        }
        public async Task<FieldOfInterestView> DeleteAsync(FieldOfInterestView field)
        {
            if (!DeleteValid(field))
            {
                throw new InvalidOperationException();
            }

            var fieldOfInterest = await _context.FieldsOfInterest.FirstOrDefaultAsync(x => (x.FieldOfInterestId == field.FieldOfInterestId));
            if (fieldOfInterest == null)
            {
                // TODO: return not found/unknown error
                throw new InvalidOperationException();
            }

            // delete main record
            _context.FieldsOfInterest.Remove(fieldOfInterest);
            // delete related records (for this simple records could also be done with an include clause on main record) 
            await _context.FieldOfInterestDescriptions.Where(x => (x.FieldOfInterestId == field.FieldOfInterestId))
                                                      .ForEachAsync(x => _context.FieldOfInterestDescriptions.Remove(x));
            await _context.SaveChangesAsync();
            return field;
        }
    }
}
