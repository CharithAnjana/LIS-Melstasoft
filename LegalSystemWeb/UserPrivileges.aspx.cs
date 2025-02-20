﻿using LegalSystemCore.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LegalSystemWeb
{
    public partial class UserPrivileges : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindUser();
            }
        }
        protected void ddlUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlUser.SelectedValue != "")
            {
                gvUserPrevilages.Visible = true;
                BindFunctionList();
            }
            else
            {
                lblUserType.Text = "";
                gvUserPrevilages.Visible = false;
            }
        }

        protected void btnChange_Click(object sender, EventArgs e)
        {
            //int rowIndex = ((GridViewRow)((LinkButton)sender).NamingContainer).RowIndex;
            //List<AutFunction> autUserFunctionList = (List<AutFunction>)ViewState["previlagesList"];

            //AutUserFunctionController autUserFunctionController = ControllerFactory.CreateAutUserFunctionController();

            //AutUserFunction autUserFunction = new AutUserFunction();
            //autUserFunction.AutUserId = Convert.ToInt32(ddlUser.SelectedValue);
            //autUserFunction.AutFunctionId = autUserFunctionList[rowIndex].AutFunctionId;

            //autUserFunctionController.Change(autUserFunction);

            //BindFunctionList();
        }

        private void BindUser()
        {
            //SystemUserController systemUserController = ControllerFactory.CreateSystemUserController();
            //List<SystemUser> userList = systemUserController.GetAllSystemUser(false, true, false);

            //ddlUser.DataSource = userList;
            //ddlUser.DataValueField = "SystemUserId";
            //ddlUser.DataTextField = "Name";
            //ddlUser.DataBind();
            //ddlUser.Items.Insert(0, new ListItem("-- select user --", ""));

            //ViewState["userList"] = userList;
        }


        private void BindFunctionList()
        {
            //List<SystemUser> userList = (List<SystemUser>)ViewState["userList"];
            //SystemUser systemUser = userList.Where(x => x.SystemUserId == Convert.ToInt32(ddlUser.SelectedValue)).Single();
            //lblUserType.Text = systemUser._UserType.UserTypeName;

            //AutFunctionController autFunctionController = ControllerFactory.CreateAutFunctionController();
            //List<AutFunction> autFunctionList = autFunctionController.GetAllAutFunction();

            //foreach (var item in autFunctionList)
            //{
            //    item.Status = "NO";
            //}

            //AutUserFunctionController autUserFunctionController = ControllerFactory.CreateAutUserFunctionController();
            //List<AutUserFunction> autUserFunctionList = autUserFunctionController.GetAllAutUserFunctionByUserId(false, Convert.ToInt32(ddlUser.SelectedValue));

            //if (autUserFunctionList.Count != 0)
            //{

            //    foreach (var item1 in autFunctionList)
            //    {
            //        foreach (var item2 in autUserFunctionList)
            //        {
            //            if (item2.AutFunctionId == item1.AutFunctionId)
            //            {
            //                item1.Status = "YES";
            //            }
            //        }
            //    }
            //}

            //gvUserPrevilages.DataSource = autFunctionList;
            //gvUserPrevilages.DataBind();

            //ViewState["previlagesList"] = autFunctionList;

        }
    }
}