using System;
using System.Collections.Generic;
using System.Linq;
using Academy.Lib.Context;
using Academy.Lib.Infrastructure;

namespace Academy.Lib.Models
{
    public class Student : Entity
    {
        #region Static Validations
               
        public static ValidationResult<string> ValidateDni(string dni, Guid currentId = default)
        {
            var output = new ValidationResult<string>()
            {
                IsSuccess = true
            };

            if (string.IsNullOrEmpty(dni))
            {
                output.IsSuccess = false;
                output.Errors.Add("El dni del alumno no puede estar vacío");
            }

            #region check duplication

            // esto para después
            //var repo = new StudentRepository();
            //var entityWithDni = repo.GetStudentByDni(dni);

            var repo = new Repository<Student>();
            var entityWithDni = repo.QueryAll().FirstOrDefault(x => x.Dni == dni);

            if (currentId == default && entityWithDni != null)
            {
                // on create
                output.IsSuccess = false;
                output.Errors.Add("Ya existe un alumno con ese dni");
            }
            else if (currentId != default && entityWithDni.Id != currentId)
            {
                // on update
                output.IsSuccess = false;
                output.Errors.Add("Ya existe un alumno con ese dni");
            }
            #endregion

            return output;
        }

        public static ValidationResult<int> ValidateChairNumber(string chairNumberText)
        {
            var output = new ValidationResult<int>()
            {
                IsSuccess = true
            };

            var chairNumber = 0;
            var isConversionOk = false;

            #region check null or empty
            if (string.IsNullOrEmpty(chairNumberText))
            {
                output.IsSuccess = false;
                output.Errors.Add("El número de la silla no puede estar vacío o nulo");
            }
            #endregion

            #region check format conversion

            isConversionOk = int.TryParse(chairNumberText, out chairNumber);

            if (!isConversionOk)
            {
                output.IsSuccess = false;
                output.Errors.Add($"No se puede convertir {chairNumber} en número");
            }

            #endregion

            #region check if the char is already in use

            if (isConversionOk)
            {
                var repoStudents = new Repository<Student>();
                var currentStudentInChair = repoStudents.QueryAll().FirstOrDefault(s => s.ChairNumber == chairNumber);

                if (currentStudentInChair != null)
                {
                    output.IsSuccess = false;
                    output.Errors.Add($"Ya hay un alumno {currentStudentInChair.Name} en la silla {chairNumber}");
                }
            }
            #endregion

            if (output.IsSuccess)
                output.ValidatedResult = chairNumber;

            return output;
        }

        public static ValidationResult<string> ValidateName(string name)
        {
            var output = new ValidationResult<string>()
            {
                IsSuccess = true
            };

            if (string.IsNullOrEmpty(name))
            {
                output.IsSuccess = false;
                output.Errors.Add("El nombre del alumno no puede estar vacío");
            }

            return output;
        }

        #endregion

        public string Dni { get; set; }
        public string Name { get; set; }

        public int ChairNumber { get; set; }

        public List<Exam> Exams
        {
            get
            {                

                var repoExams = new Repository<Exam>();

                return repoExams.QueryAll().Where(e => e.student.Id == this.Id).ToList();
            }
        }

        public Guid Guid { get; private set; }

        #region Domain Validations

        public void ValidateName(ValidationResult validationResult)
        {
            var validateNameResult = ValidateName(this.Name);
            if (!validateNameResult.IsSuccess)
            {
                validationResult.IsSuccess = false;
                validationResult.Errors.AddRange(validateNameResult.Errors);
            }
        }

        public void ValidateDni(ValidationResult validationResult)
        {            
            var vr = ValidateDni(this.Dni, this.Id);

            if (!vr.IsSuccess)
            {
                validationResult.IsSuccess = false;
                validationResult.Errors.AddRange(vr.Errors);
            }
        }

        public void ValidateChairNumber(ValidationResult validationResult)
        {
            var vr = ValidateChairNumber(this.ChairNumber.ToString());

            if (!vr.IsSuccess)
            {
                validationResult.IsSuccess = false;
                validationResult.Errors.AddRange(vr.Errors);
            }
        }

        #endregion

        public SaveResult<Student> Save()
        {            
            var saveResult = base.Save<Student>();
            return saveResult;
        }

        public override ValidationResult Validate()
        {
            var output = base.Validate();

            // cambiar ValidateName para que sea igual que ValidateDni
            ValidateName(output);
            ValidateDni(output);
            ValidateChairNumber(output);

            return output;
        }
    }
}
