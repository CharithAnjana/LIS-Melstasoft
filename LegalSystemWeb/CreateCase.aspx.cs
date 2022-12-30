﻿using LegalSystemCore;
using LegalSystemCore.Common;
using LegalSystemCore.Controller;
using LegalSystemCore.Domain;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LegalSystemWeb
{
    public partial class CreateCase : System.Web.UI.Page
    {

        int UserId, UserRoleId;
        string UserName;
        List<CourtLocation> courtlocation = new List<CourtLocation>();
        List<Court> courtList = new List<Court>();
        List<Lawyer> lawyerList = new List<Lawyer>();
        public static List<Lawyer> CounselorLawyerList = new List<Lawyer>();
        ICounselorController counselorController = ControllerFactory.CreateCounselorController();
        IPartyController partyControllerGlobal = ControllerFactory.CreatePartyController();
        IPartyCaseController partyCaseController = ControllerFactory.CreatePartyCaseController();
        public static List<Party> partyList = new List<Party>();
        Counselor counselor = new Counselor();
        public static List<Party> plaintif = new List<Party>();
        public static List<Party> defendent = new List<Party>();
        public static List<Counselor> counselorList = new List<Counselor>();
        public static string caseNumber;

        protected void Page_Load(object sender, EventArgs e)
        {


            if (Session["User_Id"] == null)
            {
                Response.Redirect("Login.aspx");
            }
            else
            {
                if (Session["User_Role_Id"].ToString() == "3" || Session["User_Role_Id"].ToString() == "2")
                    Response.Redirect("404.aspx");
                else
                {
                    if (!IsPostBack)
                    {
                        plaintif.Clear();
                        defendent.Clear();
                        partyList.Clear();
                        counselorList.Clear();
                        CounselorLawyerList.Clear();
                        if (pageSwitch())
                        {
                            hTitle.InnerText = "Update Case";
                        }
                        else
                        {
                            hTitle.InnerText = "Create Case";


                        }
                        BindCompanyList();
                        BindCaseNatureList();
                        BindCourtList();
                        BindLawyerList();
                        partyList = partyControllerGlobal.GetPartyList();

                        if (pageSwitch())
                        {
                            pageUpdateSet();
                        }
                    }
                }
            }
        }

        private void pageUpdateSet()
        {
            btnBack.Visible = false;
            btnSave.Text = "Update";

            ICaseMasterController caseMasterController = ControllerFactory.CreateCaseMasterController();
            CaseMaster caseMaster = new CaseMaster();
            caseMaster = caseMasterController.GetCaseMaster(Request.QueryString["casenumber"].ToString(), true);
            caseNumber = caseMaster.CaseNumber;
            ddlCompany.SelectedValue = caseMaster.CompanyId.ToString();
            BindCompanyUnitList();
            ddlCompanyUnit.SelectedValue = caseMaster.CompanyUnitId.ToString();
            ddlNatureOfCase.SelectedValue = caseMaster.CaseNatureId.ToString();
            txtCaseDescription.Text = caseMaster.CaseDescription;
            txtClaimAmount.Text = caseMaster.ClaimAmount.ToString();
            rbIsPlantiff.SelectedValue = caseMaster.IsPlentif.ToString();
            txtCaseOpenDate.Text = caseMaster.CaseOpenDate.ToString("yyyy-MM-dd");

            List<PartyCase> partyCaseList = new List<PartyCase>();
            partyCaseList = partyCaseController.GetPartyCaseList(caseMaster.CaseNumber);



            Party party = new Party();
            if (partyCaseList.Where(x => x.IsPlaintif == 1).Any())
            {
                foreach (PartyCase partyCase in partyCaseList.Where(x => x.IsPlaintif == 1))
                {
                    party = partyControllerGlobal.GetParty(partyCase.PartyId);
                    plaintif.Add(party);
                }
            }
            if (partyCaseList.Where(x => x.IsPlaintif == 0).Any())
            {
                foreach (PartyCase partyCase in partyCaseList.Where(x => x.IsPlaintif == 0))
                {
                    party = partyControllerGlobal.GetParty(partyCase.PartyId);
                    defendent.Add(party);
                }
            }
            BindPlaintifList();
            BindDefendentList();
            ddlCourt.SelectedValue = caseMaster.CourtId.ToString();
            BindDCourtLocationList();
            ddlLocation.SelectedValue = caseMaster.location.ToString();
            txtCaseNumber.Text = caseMaster.CaseNumber.ToString();
            if (caseMaster.PrevCaseNumber == null)
            {
                caseMaster.PrevCaseNumber = "";
            }
            txtPreCaseNumber.Text = caseMaster.PrevCaseNumber.ToString();
            ddlAttorney.SelectedValue = caseMaster.AssignAttornerId.ToString();
            counselorList = counselorController.GetCounselorList(caseMaster.CaseNumber);
            ILawyerController lawyerController = ControllerFactory.CreateLawyerController();
            Lawyer lawyer = new Lawyer();
            foreach (Counselor counselor in counselorList)
            {
                lawyer = lawyerController.GetLawyer(counselor.LawyerId);
                CounselorLawyerList.Add(lawyer);
            }

            BindCounselorList();
        }

        private bool pageSwitch()
        {
            Dictionary<string, string> allRequestParamesDictionary = Request.Params.AllKeys.ToDictionary(x => x, x => Request.Params[x]);
            if (allRequestParamesDictionary.ContainsKey("update"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void BindCompanyList()
        {
            ICompanyController companyController = ControllerFactory.CreateCompanyController();
            List<Company> companyList = companyController.GetCompanyList(false);

            int companyId = Convert.ToInt32(Session["company_id"].ToString());
            UserId = Convert.ToInt32(Session["User_Role_Id"]);

            if (UserId == 4 || UserId == 5)
                companyList = companyList.Where(c => c.CompanyId == companyId).ToList();

            ddlCompany.DataSource = companyList;
            ddlCompany.DataValueField = "CompanyId";
            ddlCompany.DataTextField = "CompanyName";
            ddlCompany.DataBind();
            ddlCompany.Items.Insert(0, new ListItem("-- select company --", ""));

        }
        private void BindCompanyUnitList()
        {
            ICompanyUnitController companyUnitController = ControllerFactory.CreateCompanyUnitController();
            if (ddlCompany.SelectedValue != "")
            {
                List<CompanyUnit> companyUnitList = companyUnitController.GetCompanyUnitListFilter(false, ddlCompany.SelectedValue);

                int companyUnitId = Convert.ToInt32(Session["company_unit_id"].ToString());
                UserId = Convert.ToInt32(Session["User_Role_Id"]);

                if (UserId == 5)
                    companyUnitList = companyUnitList.Where(c => c.CompanyUnitId == companyUnitId).ToList();

                ddlCompanyUnit.DataSource = companyUnitList;
                ddlCompanyUnit.DataValueField = "CompanyUnitId";
                ddlCompanyUnit.DataTextField = "CompanyUnitName";

                ddlCompanyUnit.DataBind();

            }
            else
            {
                ddlCompanyUnit.Items.Clear();
            }
        }
        private void BindCaseNatureList()
        {
            ICaseNatureController caseNatureController = ControllerFactory.CreateCaseNatureController();
            ddlNatureOfCase.DataSource = caseNatureController.GetCaseNatureList(false);
            ddlNatureOfCase.DataValueField = "CaseNatureId";
            ddlNatureOfCase.DataTextField = "CaseNatureName";
            ddlNatureOfCase.DataBind();
            ddlNatureOfCase.Items.Insert(0, new ListItem("-- select case nature --", ""));

        }

        private void BindCourtList()
        {

            ICourtController courtController = ControllerFactory.CreateCourtController();

            courtList = courtController.GetCourtList(false);
            ddlCourt.DataSource = courtList;
            ddlCourt.DataValueField = "CourtId";
            ddlCourt.DataTextField = "CourtName";
            ddlCourt.DataBind();
            ddlCourt.Items.Insert(0, new ListItem("-- select court --", ""));


        }
        private void BindDCourtLocationList()
        {
            ICourtLocationController courtlocationController = ControllerFactory.CreateCourtLocationController();

            if (ddlCourt.SelectedValue != "")
            {
                courtlocation = courtlocationController.GetCourtLocationListFilter(Convert.ToInt32(ddlCourt.SelectedValue));
                List<Location> listLocation = new List<Location>();
                foreach (CourtLocation courtLocation in courtlocation)
                {
                    listLocation.Add(courtLocation.location);
                }

                ddlLocation.DataSource = listLocation;
                ddlLocation.DataValueField = "LocationId";
                ddlLocation.DataTextField = "locationName";

                ddlLocation.DataBind();
            }
            else
            {
                ddlLocation.Items.Clear();
            }
        }

        private void BindLawyerList()
        {

            ILawyerController lawyerController = ControllerFactory.CreateLawyerController();
            lawyerList = lawyerController.GetLawyerList(false);

            ddlAttorney.DataSource = lawyerList;
            ddlAttorney.DataValueField = "LawyerId";
            ddlAttorney.DataTextField = "LawyerName";
            ddlAttorney.DataBind();
            ddlAttorney.Items.Insert(0, new ListItem("-- select Attorney --", ""));


            ddlCounselor.DataSource = lawyerList;
            ddlCounselor.DataValueField = "LawyerId";
            ddlCounselor.DataTextField = "LawyerName";
            ddlCounselor.DataBind();
            ddlCounselor.Items.Insert(0, new ListItem("-- select Counselor --", ""));

        }


        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (pageSwitch())
            {
                btnUpdate_Click();
            }
            else
            {
                if (CounselorLawyerList.Any() && ((plaintif.Any() && rbIsPlantiff.SelectedValue == "0") || (defendent.Any() && rbIsPlantiff.SelectedValue == "1")))
                {
                    ICaseMasterController caseMasterController = ControllerFactory.CreateCaseMasterController();
                    IPartyController partyController = ControllerFactory.CreatePartyController();
                    IPartyCaseController partyCaseController = ControllerFactory.CreatePartyCaseController();

                    CaseMaster caseMaster = new CaseMaster();

                    CultureInfo provider = new CultureInfo("en-US");

                    if (CheckAvailableCaseNum(false, txtCaseNumber.Text, caseMasterController))
                    {
                        if (CheckAvailableCaseNum(true, txtPreCaseNumber.Text, caseMasterController) || txtPreCaseNumber.Text == "")
                        {

                            caseMaster.CaseNumber = txtCaseNumber.Text;
                            caseMaster.PrevCaseNumber = txtPreCaseNumber.Text;
                            caseMaster.CompanyId = Convert.ToInt32(ddlCompany.SelectedValue);
                            caseMaster.CompanyUnitId = Convert.ToInt32(ddlCompanyUnit.SelectedValue);
                            caseMaster.CaseNatureId = Convert.ToInt32(ddlNatureOfCase.SelectedValue);
                            string test = txtCaseOpenDate.Text;
                            caseMaster.CaseOpenDate = DateTime.Parse(txtCaseOpenDate.Text, provider, DateTimeStyles.AdjustToUniversal);
                            caseMaster.CaseDescription = txtCaseDescription.Text;
                            string clamount = txtClaimAmount.Text;
                            caseMaster.ClaimAmount = txtClaimAmount.Text;
                            caseMaster.IsPlentif = Convert.ToInt32(rbIsPlantiff.Text);

                            caseMaster.CourtId = Convert.ToInt32(ddlCourt.SelectedValue);
                            caseMaster.LocationId = Convert.ToInt32(ddlLocation.SelectedValue);
                            caseMaster.AssignAttornerId = Convert.ToInt32(ddlAttorney.SelectedValue);
                            counselor.CaseNumber = caseMaster.CaseNumber;


                            caseMaster.CreatedUserId = Convert.ToInt32(Session["User_Id"]);
                            caseMaster.CreatedDate = DateTime.Now;
                            caseMaster.CaseStatusId = 1;
                            caseMaster.CreatedUserId = Convert.ToInt32(Session["User_Id"]);
                            caseMaster.CreatedDate = DateTime.Now;
                            caseMaster.CaseStatusId = 1;

                            caseMasterController.Save(caseMaster);

                            foreach (Lawyer lawyer in CounselorLawyerList)
                            {
                                counselor.LawyerId = lawyer.LawyerId;
                                counselorController.Save(counselor);
                            }
                            PartyCase partyCase = new PartyCase();

                            foreach (Party party in plaintif)
                            {
                                if (partyList.Where(x => x.PartyName == party.PartyName).Any())
                                {
                                    partyCase.PartyId = partyList.Where(x => x.PartyName == party.PartyName).ElementAt(0).PartyId;
                                }
                                else
                                {
                                    partyCase.PartyId = partyController.Save(party);
                                }

                                partyCase.CaseNumber = caseMaster.CaseNumber;
                                partyCase.IsPlaintif = 1;
                                partyCaseController.Save(partyCase);
                            }


                            foreach (Party party in defendent)
                            {
                                if (partyList.Where(x => x.PartyName == party.PartyName).Any())
                                {
                                    partyCase.PartyId = partyList.Where(x => x.PartyName == party.PartyName).ElementAt(0).PartyId;
                                }
                                else
                                {
                                    partyCase.PartyId = partyController.Save(party);
                                }
                                partyCase.CaseNumber = caseMaster.CaseNumber;
                                partyCase.IsPlaintif = 0;
                                partyCaseController.Save(partyCase);
                            }

                            UploadFiles();
                            Clear();
                            lblSuccessMsg.Text = "Record Updated Successfully!";
                        }
                    }

                    Clear();
                    clearCounselor();
                    clearDefendent();
                    clearPlaintif();
                    lblSuccessMsg.Text = "Record Updated Successfully!";
                }
                else
                {

                }
                if (!CounselorLawyerList.Any())
                {
                    lblCounselor.Text = "Please Add Counselor";
                }
                if (!(plaintif.Any() && rbIsPlantiff.SelectedValue == "0"))
                {


                    lblPlaintif.Text = "Please Add Plaintif Side";
                }
                if (!(defendent.Any() && rbIsPlantiff.SelectedValue == "1"))
                {
                    lblDefendent.Text = "Please Add Defendent Side";
                }
            }
        }

        protected void btnUpdate_Click()
        {
            if (CounselorLawyerList.Any() && ((plaintif.Any() && rbIsPlantiff.SelectedValue == "0") || (defendent.Any() && rbIsPlantiff.SelectedValue == "1")))
            {
                ICaseMasterController caseMasterController = ControllerFactory.CreateCaseMasterController();
                IPartyController partyController = ControllerFactory.CreatePartyController();
                IPartyCaseController partyCaseController = ControllerFactory.CreatePartyCaseController();

                CaseMaster caseMaster = new CaseMaster();

                CultureInfo provider = new CultureInfo("en-US");

                if (CheckAvailableCaseNum(false, txtCaseNumber.Text, caseMasterController) || txtCaseNumber.Text == caseNumber)
                {
                    if (CheckAvailableCaseNum(true, txtPreCaseNumber.Text, caseMasterController) || txtCaseNumber.Text == caseNumber || txtPreCaseNumber.Text == "")
                    {
                        caseMaster.PrevCaseNumberUpdate = caseNumber;
                        caseMaster.CaseNumber = txtCaseNumber.Text;
                        caseMaster.PrevCaseNumber = txtPreCaseNumber.Text;
                        caseMaster.CompanyId = Convert.ToInt32(ddlCompany.SelectedValue);
                        caseMaster.CompanyUnitId = Convert.ToInt32(ddlCompanyUnit.SelectedValue);
                        caseMaster.CaseNatureId = Convert.ToInt32(ddlNatureOfCase.SelectedValue);
                        string test = txtCaseOpenDate.Text;
                        caseMaster.CaseOpenDate = DateTime.Parse(txtCaseOpenDate.Text, provider, DateTimeStyles.AdjustToUniversal);
                        caseMaster.CaseDescription = txtCaseDescription.Text;
                        string clamount = txtClaimAmount.Text;
                        caseMaster.ClaimAmount = txtClaimAmount.Text;
                        caseMaster.IsPlentif = Convert.ToInt32(rbIsPlantiff.Text);

                        caseMaster.CourtId = Convert.ToInt32(ddlCourt.SelectedValue);
                        caseMaster.LocationId = Convert.ToInt32(ddlLocation.SelectedValue);
                        caseMaster.AssignAttornerId = Convert.ToInt32(ddlAttorney.SelectedValue);
                        counselor.CaseNumber = caseMaster.CaseNumber;
                        if (txtPreCaseNumber.Text == null || txtPreCaseNumber.Text == "")
                        {
                            caseMaster.PrevCaseNumber = "";
                        }
                        else
                        {
                            caseMaster.PrevCaseNumber = txtPreCaseNumber.Text;
                        }


                        caseMaster.CreatedUserId = Convert.ToInt32(Session["User_Id"]);
                        caseMaster.CreatedDate = DateTime.Now;
                        caseMaster.CaseStatusId = 1;
                        caseMaster.CreatedUserId = Convert.ToInt32(Session["User_Id"]);
                        caseMaster.CreatedDate = DateTime.Now;
                        caseMaster.CaseStatusId = 1;

                        caseMasterController.Update(caseMaster);
                        if (counselorController.GetCounselorList(caseMaster.CaseNumber).Any())
                        {
                            counselorController.DeletePermenent(caseMaster.CaseNumber);
                        }

                        if (counselorList.Any())
                        {
                            foreach (Counselor counselor in counselorList)
                            {

                                counselorController.Save(counselor);
                            }
                        }

                        PartyCase partyCase = new PartyCase();
                        if (partyCaseController.GetPartyCaseList(caseMaster.CaseNumber).Any())
                        {
                            partyCaseController.DeletePermenent(caseMaster.CaseNumber);
                        }

                        foreach (Party party in plaintif)
                        {
                            if (partyList.Where(x => x.PartyName == party.PartyName).Any())
                            {
                                partyCase.PartyId = partyList.Where(x => x.PartyName == party.PartyName).ElementAt(0).PartyId;
                            }
                            else
                            {
                                partyCase.PartyId = partyController.Save(party);
                            }

                            partyCase.CaseNumber = caseMaster.CaseNumber;
                            partyCase.IsPlaintif = 1;
                            partyCaseController.Save(partyCase);
                        }


                        foreach (Party party in defendent)
                        {
                            if (partyList.Where(x => x.PartyName == party.PartyName).Any())
                            {
                                partyCase.PartyId = partyList.Where(x => x.PartyName == party.PartyName).ElementAt(0).PartyId;
                            }
                            else
                            {
                                partyCase.PartyId = partyController.Save(party);
                            }
                            partyCase.CaseNumber = caseMaster.CaseNumber;
                            partyCase.IsPlaintif = 0;
                            partyCaseController.Save(partyCase);
                        }


                        UploadFiles();
                        Clear();
                        lblSuccessMsg.Text = "Record Updated Successfully!";
                    }
                }

                Clear();
                clearCounselor();
                clearDefendent();
                clearPlaintif();
                lblSuccessMsg.Text = "Record Updated Successfully!";
            }
            else
            {

            }
            if (!counselorList.Any())
            {
                lblCounselor.Text = "Please Add Counselor";
            }
            if ((!plaintif.Any() && rbIsPlantiff.SelectedValue == "1"))
            {


                lblPlaintif.Text = "Please Add Plaintif Side";
            }
            if ((!defendent.Any() && rbIsPlantiff.SelectedValue == "0"))
            {
                lblDefendent.Text = "Please Add Defendent Side";
            }
        }



        protected void btnDocUpload_Click1(object sender, EventArgs e)
        {
            Response.Redirect("UploadDocument.aspx");
        }


        private void UploadFiles()
        {
            IDocumentController documentController = ControllerFactory.CreateDocumentController();
            IDocumentCaseController documentCaseController = ControllerFactory.CreateDocumentCaseController();

            Document document = new Document();
            DocumentCase documentCase = new DocumentCase();

            if (Uploader.HasFile)
            {
                HttpFileCollection uploadFiles = Request.Files;
                for (int i = 0; i < uploadFiles.Count; i++)
                {
                    HttpPostedFile uploadFile = uploadFiles[i];
                    if (uploadFile.ContentLength > 0)
                    {
                        uploadFile.SaveAs(Server.MapPath("~/SystemDocuments/CaseMaster/") + uploadFile.FileName);
                        //lblListOfUploadedFiles.Text += String.Format("{0}<br />", uploadFile.FileName);

                        document.DocumentType = "case";
                        documentCase.DocumentId = documentController.Save(document);

                        documentCase.DocumentName = uploadFile.FileName;
                        documentCase.CaseNumber = txtCaseNumber.Text;
                        documentCase.DocumentDescription = "";
                        documentCaseController.Save(documentCase);
                    }
                }
            }
        }

        protected void ddlCourt_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindDCourtLocationList();
        }


        protected void ddlCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindCompanyUnitList();
        }

        private void Clear()
        {
            txtCaseDescription.Text = string.Empty;
            txtCaseNumber.Text = string.Empty;
            txtClaimAmount.Text = string.Empty;
            txtPreCaseNumber.Text = string.Empty;
            ddlCompany.SelectedIndex = 0;
            ddlNatureOfCase.SelectedIndex = 0;
            ddlCourt.SelectedIndex = 0;
            ddlAttorney.SelectedIndex = 0;
            ddlCounselor.SelectedIndex = 0;
            ddlCompanyUnit.Items.Clear();
            ddlLocation.Items.Clear();
            txtCaseOpenDate.Text = string.Empty;
            rbIsPlantiff.Items[0].Selected = false;
            rbIsPlantiff.Items[1].Selected = false;
            lblCaseNumberError.Text = string.Empty;
            lblPrevCaseNumberError.Text = string.Empty;
            lblCounselor.Text = string.Empty;
            lblDefendent.Text = string.Empty;
            lblPlaintif.Text = string.Empty;
            txtPlaintif.Text = string.Empty;
            txtDefendent.Text = string.Empty;
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private bool CheckAvailableCaseNum(bool isPrev, string Number, ICaseMasterController c)
        {
            CaseMaster caseMaster = c.GetCaseMaster(Number, false);



            if (caseMaster.CaseNumber == null)
            {
                if (isPrev)
                {
                    lblSuccessMsg.Text = string.Empty;
                    lblCaseNumberError.Text = string.Empty;
                    lblPrevCaseNumberError.Text = "Not a Valid Case Number";
                    return false;
                }
                else
                {
                    lblCaseNumberError.Text = string.Empty;
                    lblSuccessMsg.Text = string.Empty;
                    lblPrevCaseNumberError.Text = string.Empty;
                    return true;
                }
            }
            else
            {
                if (isPrev)
                {
                    lblPrevCaseNumberError.Text = string.Empty;
                    lblSuccessMsg.Text = string.Empty;
                    lblCaseNumberError.Text = string.Empty;
                    return true;
                }
                else
                {
                    lblCaseNumberError.Text = "Case Number Already Exsists!";
                    lblPrevCaseNumberError.Text = string.Empty;
                    lblSuccessMsg.Text = string.Empty;
                    return false;
                }
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            ILawyerController lawyerController = ControllerFactory.CreateLawyerController();
            Lawyer lawyer = new Lawyer();

            lawyer.LawyerName = ddlCounselor.SelectedItem.Text;
            if (ddlCounselor.SelectedIndex != 0)
            {
                lawyer.LawyerId = Convert.ToInt32(ddlCounselor.SelectedValue);
                string lawyerName = ddlCounselor.SelectedItem.Text;
                if (!(CounselorLawyerList.Where(x => x.LawyerName == lawyerName).Any()))
                {
                    CounselorLawyerList.Add(lawyer);
                    BindCounselorList();
                    if (lblCounselor.Text != "")
                    {
                        lblCounselor.Text = "";
                    }
                }
            }


        }

        protected void btnAddPlaintif_Click(object sender, EventArgs e)
        {
            Party party = new Party();
            party.PartyName = txtPlaintif.Text;
            if (!(plaintif.Where(x => x.PartyName == party.PartyName).Any()) && party.PartyName != "" && !(defendent.Where(x => x.PartyName == party.PartyName).Any()))
            {
                plaintif.Add(party);
                BindPlaintifList();
                if (lblPlaintif.Text != "")
                {
                    lblPlaintif.Text = "";
                }
            }
        }

        protected void btnAddDefendent_Click(object sender, EventArgs e)
        {
            Party party = new Party();
            party.PartyName = txtDefendent.Text;
            if (!(defendent.Where(x => x.PartyName == party.PartyName).Any()) && party.PartyName != "" && !(plaintif.Where(x => x.PartyName == party.PartyName).Any()))
            {
                defendent.Add(party);
                BindDefendentList();
                if (lblDefendent.Text != "")
                {
                    lblDefendent.Text = "";
                }
            }
        }

        protected void GridView2_OnPageIndexChanged(object sender, GridViewPageEventArgs e)
        {
            gvCounselor.PageIndex = e.NewPageIndex;
            gvCounselor.DataSource = CounselorLawyerList;
            gvCounselor.DataBind();
        }

        private void BindCounselorList()
        {
            gvCounselor.DataSource = CounselorLawyerList;
            gvCounselor.DataBind();

        }

        private void BindPlaintifList()
        {
            gvPlaintif.DataSource = plaintif;
            gvPlaintif.DataBind();
        }

        private void BindDefendentList()
        {
            gvDefendent.DataSource = defendent;
            gvDefendent.DataBind();
        }

        protected void btndelete_Click(object sender, EventArgs e)
        {
            int rowIndex = ((GridViewRow)((LinkButton)sender).NamingContainer).RowIndex;
            int pageSize = gvCounselor.PageSize;
            int pageIndex = gvCounselor.PageIndex;

            rowIndex = (pageSize * pageIndex) + rowIndex;

            CounselorLawyerList.RemoveAll(x => x.LawyerName == CounselorLawyerList[rowIndex].LawyerName);
            BindCounselorList();

        }

        protected void btndelete_ClickPlaintif(object sender, EventArgs e)
        {
            int rowIndex = ((GridViewRow)((LinkButton)sender).NamingContainer).RowIndex;
            int pageSize = gvPlaintif.PageSize;
            int pageIndex = gvPlaintif.PageIndex;

            rowIndex = (pageSize * pageIndex) + rowIndex;

            plaintif.RemoveAll(x => x.PartyName == plaintif[rowIndex].PartyName);
            BindPlaintifList();

        }

        protected void btndelete_ClickDefendent(object sender, EventArgs e)
        {
            int rowIndex = ((GridViewRow)((LinkButton)sender).NamingContainer).RowIndex;
            int pageSize = gvDefendent.PageSize;
            int pageIndex = gvDefendent.PageIndex;

            rowIndex = (pageSize * pageIndex) + rowIndex;

            defendent.RemoveAll(x => x.PartyName == defendent[rowIndex].PartyName);
            BindDefendentList();

        }

        private void clearCounselor()
        {
            CounselorLawyerList.Clear();
            BindCounselorList();
        }
        private void clearPlaintif()
        {
            plaintif.Clear();
            BindPlaintifList();
        }

        private void clearDefendent()
        {
            defendent.Clear();
            BindDefendentList();
        }



        public static string ConvertNumbertoWords(long number)
        {
            var unitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
            var tensMap = new[] { "Zero", "Ten", "Twenty", "Thirty", "Fourty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
            if (number == 0)
                return "Zero";
            if (number < 0)
                return "minus " + ConvertNumbertoWords(Math.Abs(number));
            string words = "";
            if ((number / 1000000000) > 0)
            {
                words += ConvertNumbertoWords(number / 1000000000) + " Billion ";
                number %= 1000000000;
            }
            if ((number / 1000000) > 0)
            {
                words += ConvertNumbertoWords(number / 1000000) + " Million ";
                number %= 1000000;
            }
            if ((number / 1000) > 0)
            {
                words += ConvertNumbertoWords(number / 1000) + " Thousand ";
                number %= 1000;
            }
            if ((number / 100) > 0)
            {
                words += ConvertNumbertoWords(number / 100) + " Hundred ";
                number %= 100;
            }

            if (number > 0)
            {

                //if (words != "")
                //    words += "AND ";
                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += " " + unitsMap[number % 10];
                }
            }
            //if (number > 0)
            //{
            //    if (words != "")
            //        words += "AND ";
            //    if (number < 20)
            //        words += unitsMap[number];
            //    else
            //    {
            //        words += tensMap[number / 10];
            //        if ((number % 10) > 0)
            //            words += " " + unitsMap[number % 10];
            //    }
            //    words += " Cents";
            //}
            return words;
        }
        protected void claimAmountInWords(object sender, EventArgs e)
        {
            if ((txtClaimAmount.Text.All(x => ".0123456789".Contains(x)) || txtClaimAmount.Text.All(x => "0123456789".Contains(x))) && ((txtClaimAmount.Text.Count(x => x == '.') == 1) || (txtClaimAmount.Text.Count(x => x == '.') == 0)))
            {
                lblClaimAmountInWords.Text = "Claim Amount In Words : ";
                string number = txtClaimAmount.Text.Split('.')[0];


                //long claimAmount = long.Parse(txtClaimAmount.Text);

                lblClaimAmountInWords.Text += ConvertNumbertoWords(long.Parse(number));
                if (txtClaimAmount.Text.Contains('.'))
                {
                    string decimalNumber = txtClaimAmount.Text.Split('.')[1].Substring(0, 2);
                    lblClaimAmountInWords.Text += " And " + ConvertNumbertoWords(long.Parse(decimalNumber)) + " Cents";
                }
            }
            else
            {
                lblClaimAmountInWords.Text = "Enter Valid Amount";
            }
        }
    }
}