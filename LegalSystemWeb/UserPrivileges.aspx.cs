﻿using LegalSystemCore.Common;
using LegalSystemCore.Controller;
using LegalSystemCore.Domain;
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
        IUserRolePrivilegeController userRolePrivilegeController = ControllerFactory.CreateUserRolePrivilegeController();
        IUserRoleController userRoleController = ControllerFactory.CreateUserRoleController();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!userRolePrivilegeController.GetUserRolePrivilegeListByRole(Session["User_Role_Id"].ToString()).Where(x => x.FunctionId == 22).Any())
                    Response.Redirect("404.aspx");
                else
                {
                    BindUser();
                }
            }
        }
        protected void ddlUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlUser.SelectedValue != "")
            {
                gvUserPrevilages.Visible = true;
                lblUserType.Text = userRoleController.GetUserRole(Convert.ToInt32(Session["User_Role_Id"])).RoleName;
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

            IUserLoginController userLoginController = ControllerFactory.CreateUserLoginController();
            List<UserLogin> userLogins = userLoginController.GetUserLoginList(true);
            ddlUser.DataSource = userLogins;
            ddlUser.DataValueField = "UserId";
            ddlUser.DataTextField = "UserName";
            ddlUser.DataBind();
            ddlUser.Items.Insert(0, new ListItem("-- select user --", ""));
            ViewState["userLogins"] = userLogins;
        }


        private void BindFunctionList()
        {

            List<UserLogin> userLogins = (List<UserLogin>)ViewState["userLogins"];
            UserLogin userLogin = userLogins.Where(x => x.UserId == Convert.ToInt32(ddlUser.SelectedValue)).Single();
            lblUserType.Text = userRoleController.GetUserRole(userLogin.UserRoleId).RoleName;
            IFunctionsController functionsController = ControllerFactory.CreateFunctionsController();
            IUserRolePrivilegeController userRolePrivilegeController = ControllerFactory.CreateUserRolePrivilegeController();
            IUserPrivilegeController userPrivilegeController = ControllerFactory.CreateUserPrivilegeController();
            List<Functions> functions = functionsController.GetFunctionList();

            List<UserRolePrivilege> userRolePrivileges = userRolePrivilegeController.GetUserRolePrivilegeListByRole(userLogin.UserRoleId.ToString());
            List<UserPrivilege> UserPrivileges = userPrivilegeController.GetUserPrivilegeList(userLogin.UserId);

            foreach (var item in functions)
            {
                if (userRolePrivileges.Any(x => x.FunctionId == item.FunctionId) || UserPrivileges.Any(x => x.FunctionId == item.FunctionId && x.IsGrantRevoke == 1))
                {
                    item.Status = "Yes";
                    if (UserPrivileges.Any(x => x.FunctionId == item.FunctionId && x.IsGrantRevoke == 0))
                    {
                        item.Status = "NO";
                    }
                }
                else
                {
                    item.Status = "NO";
                }
            }

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

            gvUserPrevilages.DataSource = functions;
            gvUserPrevilages.DataBind();

            ViewState["functions"] = functions;

        }
    }
}