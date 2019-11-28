using Academy.Lib.Infrastructure;
using Academy.Lib.Models;
using System.Collections.Generic;

namespace Academy.Lib.Context
{
    public class StudentRepository : Repository<Student>
    {
        private static Dictionary<string, Student> StudentsByDni { get; set; } = new Dictionary<string, Student>();

        public override SaveResult<Student> Add(Student entity)
        {
            var output = base.Add(entity);

            if (output.IsSuccess)
            {
                StudentsByDni.Add(output.Entity.Dni, output.Entity);
            }

            return output;
        }

        public override SaveResult<Student> Update(Student entity)
        {
            var output = base.Update(entity);

            if (output.IsSuccess)
            {
                StudentsByDni[output.Entity.Dni] = output.Entity;
            }

            return output;
        }

        public Student GetStudentByDni(string dni)
        {
            if (StudentsByDni.ContainsKey(dni))
                return StudentsByDni[dni];

            return null;
        }
    }
}
