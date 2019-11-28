using Academy.Lib.Models;
using System.Collections.Generic;

namespace Academy.Lib.Infrastructure
{
    public class SaveResult<T> where T : Entity
    {
        public ValidationResult Validation { get; set; } = new ValidationResult();

        public bool IsSuccess
        {
            get
            {
                return Validation.IsSuccess;
            }
            set
            {
                Validation.IsSuccess = value;
            }
        }

        public string AllErrors
        {
            get
            {
                return Validation.AllErrors;
            }
        }

        public T Entity { get; set; }

        public SaveResult<TOut> Cast<TOut>() where TOut : Entity
        {
            var output = new SaveResult<TOut>
            {
                Entity = this.Entity as TOut,
                Validation = this.Validation
            };

            return output;
        }
    }
}
