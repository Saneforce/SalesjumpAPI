namespace WebApplicationApi.Models
{   
    public class ProductMasters
    {
        public string? ProductCode { get; set; }

        public string? ProductName { get; set; }

        public string? Short_Name { get; set; }

        public string? ProductDescription { get; set; }

        public string? ConversionFactor { get; set; }

        public string? Grossweight { get; set; }

        public string? Netweight { get; set; }

        public string? UOM { get; set; }

        public string? Base_UOM { get; set; }

        public string? Sub_Division_Name { get; set; }

        public string? ERP_Code { get; set; }

        public string? Brand { get; set; }

        public string? Product_Cat_Name { get; set; }
    }

    public class SalesForceMaster
    {
        public string? Employee_Id { get; set; }

        public string? Employee_Name { get; set; }

        public string? Designation { get; set; }

        public string? Sf_HQ { get; set; }

        public string? StateName { get; set; }

        public string? MobileNumber { get; set; }

        public string? Manager_Name { get; set; }

        public string? Territory { get; set; }

        public string? DOB { get; set; }

        public string? Total_Beats { get; set; }

        public string? DOJ { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }

        public string? Status { get; set; }

        public string? Appversion { get; set; }

    }

    public class DistributorMaster
    {
        public string? Distributor_Code { get; set; }

        public string? ERP_Code { get; set; }

        public string? Distributor_Name { get; set; }

        public string? ContactPerson { get; set; }

        public string? Address { get; set; }

        public string? Mobile { get; set; }

        public string? FieldForce_Name { get; set; }

        public string? Territory { get; set; }

        public string? Dist_Name { get; set; }       

        public string? Taluk_Name { get; set; }

        public string? StateName { get; set; }

        public string? CategoryName { get; set; }

        public string? Type { get; set; }

        public string? EmailID { get; set; }       

        public string? GSTN { get; set; }

        public string? Vendor_Code { get; set; }

        public string? Username { get; set; }

    }

    public class RouteMaster
    {
        public string? Territory_Code { get; set; }

        public string? Route_Code { get; set; }

        public string? Route_Name { get; set; }

        public string? Target { get; set; }

        public string? Territory_name { get; set; }

        public string? Create_Date { get; set; }

        public string? StateName { get; set; }

        public string? SF_Name { get; set; }

        public string? Emp_Id { get; set; }

        public string? Distributor_Name { get; set; }


    }


    public class RetailerMaster
    {
        public string? Created_Date { get; set; }

        public string? Retailer_code { get; set; }

        public string? Retailer_Name { get; set; }

        public string? ContactPerson { get; set; }

        public string? Mobile_No { get; set; }

        public string? DOA { get; set; }

        public string? DOB { get; set; }

        public string? Retailer_Channel { get; set; }

        public string? Retailer_Class { get; set; }

        public string? Route_Name { get; set; }

        public string? Territory_Name { get; set; }

        public string? AreaName { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? PinCode { get; set; }

        public string? StateName { get; set; }

        public string? FiledForce { get; set; }

        public string? HQ { get; set; }

        public string? Designation { get; set; }

        public string? DistributorName { get; set; }

        public string? GSTNO { get; set; }

        public string? ERP_Code { get; set; }

        public string? Latitude { get; set; }

        public string? Longitude { get; set; }

        public string? Profilepic { get; set; }


    }
}
