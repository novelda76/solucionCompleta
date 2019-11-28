using Academy.Lib.Context;
using Academy.Lib.Infrastructure;
using System;
using System.Linq;

namespace Academy.Lib.Models
{
    public class Subject : Entity
    {
        public string Name { get; set; }

        public static ValidationResult<string> ValidateName(string name)
        {
            var output = new ValidationResult<string>()
            {
                IsSuccess = true
            };

            if (!string.IsNullOrEmpty(name))
            {
                output.IsSuccess = false;
                output.Errors.Add("el nombre de la asignatura no puede estar vacío");
            }
            var repo = new Repository<Subject>();
            var entityWithName = repo.QueryAll().FirstOrDefault(x => x.Name == name);

            if (entityWithName != null)
            {
                output.IsSuccess = false;
                output.Errors.Add($"Ya existe una asignatura que se llama {name}");
            }

            return output;
        }

        public void ValidateName(ValidationResult validationResult)
        {
            var validateNameResult = ValidateName(this.Name);
            if (!validateNameResult.IsSuccess)
            {
                validationResult.IsSuccess = false;
                validationResult.Errors.AddRange(validateNameResult.Errors);
            }
        }

        public override ValidationResult Validate()
        {
            var output = base.Validate();

            ValidateName(output);

            return output;
        }

        public SaveResult<Subject> Save()
        {
            var saveResult = base.Save<Subject>();
            return saveResult.Cast<Subject>();
        }

       

    }
}
