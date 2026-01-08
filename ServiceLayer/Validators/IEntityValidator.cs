using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Validators
{
    /// <summary>
    /// Interface for entity validators that enforce business rules
    /// </summary>
    /// <typeparam name="T">Entity type to validate</typeparam>
    public interface IEntityValidator<T>
    {
        /// <summary>
        /// Validates an entity according to business rules
        /// </summary>
        /// <param name="entity">Entity to validate</param>
        /// <exception cref="ArgumentException">Thrown when validation fails</exception>
        /// <exception cref="ArgumentNullException">Thrown when entity is null</exception>
        void Validate(T entity);
    }
}
