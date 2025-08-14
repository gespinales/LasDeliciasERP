using Antlr.Runtime.Misc;
using LasDeliciasERP.AccesoADatos;
using LasDeliciasERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LasDeliciasERP.Pages.Customer
{
    public partial class CustomerForm : Page
    {
        CustomerDAL dalCustomer = new CustomerDAL();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string idStr = Request.QueryString["id"];
                string action = Request.QueryString["action"] ?? "save";
                hfAction.Value = action;

                if (!string.IsNullOrEmpty(idStr))
                {
                    int id = int.Parse(idStr);
                    hfId.Value = idStr;
                    LoadCustumer(id);

                    if (action == "delete")
                    {
                        formTitle.InnerText = "Eliminar Cliente";
                        btnSave.Text = "Eliminar";

                        // Opcional: deshabilitar campos para evitar edición
                        txtName.Enabled = false;
                        txtPhone.Enabled = false;
                        txtEmail.Enabled = false;
                        txtAddress.Enabled = false;
                        txtNotes.Enabled = false;
                    }
                    else
                    {
                        formTitle.InnerText = "Editar Cliente";
                        btnSave.Text = "Actualizar";
                    }
                }
                else
                {
                    formTitle.InnerText = "Nuevo Cliente";
                    btnSave.Text = "Guardar";
                }
            }
        }

        private void LoadCustumer(int id)
        {
            var customer = dalCustomer.GetById(id);
            if (customer != null)
            {
                hfId.Value = customer.Id.ToString();
                txtName.Text = customer.Name;
                txtPhone.Text = customer.Phone;
                txtEmail.Text = customer.Email;
                txtAddress.Text = customer.Address;
                txtNotes.Text = customer.Notes;
            }
            else
            {
                Response.Redirect("CustomerList.aspx");
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // Verificar validaciones del lado del servidor
            if (!Page.IsValid)
                return;

            string action = hfAction.Value;

            if (action == "delete")
            {
                // Eliminar cliente
                if (!string.IsNullOrEmpty(hfId.Value))
                {
                    int id = int.Parse(hfId.Value);
                    dalCustomer.Delete(id);
                }
            }
            else
            {
                var customer = new Models.Customer
                {
                    Name = txtName.Text.Trim(),
                    Phone = txtPhone.Text.Trim(),
                    Email = txtEmail.Text.Trim(),
                    Address = txtAddress.Text.Trim(),
                    Notes = txtNotes.Text.Trim()
                };

                if (!string.IsNullOrEmpty(hfId.Value))
                {
                    // Actualizar
                    customer.Id = int.Parse(hfId.Value);
                    dalCustomer.Update(customer);
                }
                else
                {
                    // Insertar nuevo
                    dalCustomer.Insert(customer);
                }
            }

            Response.Redirect("CustomerList.aspx");
        }
    }
}