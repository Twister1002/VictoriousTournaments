using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication.Interfaces
{
    public interface IViewModel<T> where T : class
    {
        /// <summary>
        /// Sets up the view model with preliminary information
        /// </summary>
        void SetupViewModel();
        /// <summary>
        /// Applies the changes from the submitted model to the Model
        /// </summary>
        /// <param name="viewModel">The model from the submitted form</param>
        void ApplyChanges(T viewModel);
        /// <summary>
        /// Sets the fields from the Model to the viewModel.
        /// </summary>
        void SetFields();
    }
}