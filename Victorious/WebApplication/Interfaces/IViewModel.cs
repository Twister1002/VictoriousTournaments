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
        /// <param name="viewModel">A viewmodel with data all ready provided.</param>
        /// </summary>
        void SetupViewModel(T viewModel = null);
        /// <summary>
        /// Applies the changes from the submitted model to the Model
        /// </summary>
        /// <param name="viewModel">The model from the submitted form</param>
        /// <returns>True if changed were applied successfully.</returns>
        bool ApplyChanges(T viewModel);
        /// <summary>
        /// Sets the fields from the Model to the viewModel.
        /// </summary>
        void SetFields();
    }
}