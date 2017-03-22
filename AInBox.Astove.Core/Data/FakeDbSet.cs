using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace AInBox.Astove.Core.Data
{
    public class FakeDbSet<T> : IDbSet<T> where T : class, IEntity
    {
        ObservableCollection<T> _data;
        IQueryable _query;

        public FakeDbSet(EnumerableQuery<T> collection)
        {
            _data = new ObservableCollection<T>(collection);
            _query = collection;
        }

        public virtual T Find(params object[] keyValues)
        {
            throw new NotImplementedException("Derive from FakeDbSet<T> and override Find");
        }

        public T Add(T item)
        {
            var errorMessage = Validate(item);
            if (!string.IsNullOrEmpty(errorMessage))
                throw new ValidationException(errorMessage);
            
            _data.Add(item);
            return item;
        }

        public T Remove(T item)
        {
            _data.Remove(item);
            return item;
        }

        public T Attach(T item)
        {
            var errorMessage = Validate(item);
            if (!string.IsNullOrEmpty(errorMessage))
                throw new ValidationException(errorMessage);

            _data.Add(item);
            return item;
        }

        public T Detach(T item)
        {
            _data.Remove(item);
            return item;
        }

        public T Create()
        {
            return Activator.CreateInstance<T>();
        }

        public string Validate(T entity)
        {
            var context = new ValidationContext(entity);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(entity, context, results, true);

            if (isValid)
                return string.Empty;
            else
            {
                var errorFormat = "Property: {0} - Message: {1}";
                var errors = new List<string>();
                foreach (var result in results)
                    errors.Add(string.Format(errorFormat, result.MemberNames, result.ErrorMessage));

                var errorMessage = string.Join(", ", errors.ToArray());
                return errorMessage;
            }
        }

        public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T
        {
            return Activator.CreateInstance<TDerivedEntity>();
        }

        public ObservableCollection<T> Local
        {
            get { return _data; }
        }

        Type IQueryable.ElementType
        {
            get { return _query.ElementType; }
        }

        System.Linq.Expressions.Expression IQueryable.Expression
        {
            get { return _query.Expression; }
        }

        IQueryProvider IQueryable.Provider
        {
            get { return _query.Provider; }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _data.GetEnumerator();
        }
    }
}
