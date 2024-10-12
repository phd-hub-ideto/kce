namespace KhumaloCraft.Application.Validation
{
    public class ValidationErrors<T> where T : Enum
    {
        private readonly Dictionary<T, List<string>> _errors;

        public ValidationErrors()
        {
            _errors = new Dictionary<T, List<string>>();
        }

        public void Add(T field, string error)
        {
            if (_errors.TryGetValue(field, out var errors))
            {
                errors.Add(error);
                _errors[field] = errors;
            }
            else
            {
                _errors.Add(field, new List<string> { error });
            }
        }

        public bool TryGetErrors(T field, out List<string> errors)
        {
            return _errors.TryGetValue(field, out errors);
        }

        public List<KeyValuePair<T, List<string>>> GetErrors()
        {
            return _errors.ToList();
        }

        public IEnumerable<string> GetErrorMessages()
        {
            foreach (var error in _errors)
            {
                foreach (var errorMessage in error.Value)
                {
                    yield return errorMessage;
                }
            }
        }

        public bool HasErrors()
        {
            return _errors.Count > 0;
        }
    }
}