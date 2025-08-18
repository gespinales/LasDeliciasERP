using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LasDeliciasERP.AccesoADatos;
using LasDeliciasERP.Models;

namespace LasDeliciasERP.Pages.Bird
{
    public partial class BirdDeathList : Page
    {
        private readonly BirdDeathDAL dalDeath = new BirdDeathDAL();

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadDeaths();
        }

        private void LoadDeaths()
        {
            var deaths = dalDeath.GetAll(); // Obtener todos los decesos
            gvDeaths.DataSource = deaths.Select(d => new
            {
                d.Id,
                DeathDate = d.DeathDate.ToString("yyyy-MM-dd"),
                d.BarnName,
                d.BirdTypeName,
                d.Quantity,
                d.Reason
            }).ToList();
            gvDeaths.DataBind();
        }

        protected void gvDeaths_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvDeaths.PageIndex = e.NewPageIndex;
            LoadDeaths();
        }
    }
}