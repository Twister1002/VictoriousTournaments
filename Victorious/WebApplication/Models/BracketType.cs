﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DatabaseLib;

namespace WebApplication.Models
{
    public class BracketType : Model
    {
        public List<BracketTypeModel> Brackets { get; private set; }

        public BracketType(IUnitOfWork work) : base(work)
        {
            Init();
        }

        private void Init()
        {
            Retrieve();
        }

        #region CRUD
        public bool Create()
        {
           throw new NotSupportedException("Not able to create a bracket");
        }

        public void Retrieve()
        {
            Brackets = services.Type.GetAllBracketTypes();
        }

        public bool Update(int id)
        {
            BracketTypeModel model = services.Type.GetBracketType(id);
            model.IsActive = model.IsActive ? false : true;

            services.Type.UpdateBracketType(model);
            return services.Save();
        }

        public bool Delete()
        {
            throw new NotSupportedException("Not able to delete a bracket");
        }
        #endregion
    }
}