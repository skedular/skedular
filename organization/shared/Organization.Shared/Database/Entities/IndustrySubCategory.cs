using Api.Shared.Services;
using Enterprise.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Organization.Shared.Database.Entities;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class IndustrySubCategory : EntityBaseWithDeleted
{
    public string Name { get; set; }

    public virtual IndustryMainCategory IndustryMainCategory { get; set; }
    public string IndustryMainCategoryId { get; set; }
    public virtual ICollection<Organization> Organizations { get; set; } = [];
}
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

public class IndustrySubCategoryConfiguration : IEntityTypeConfiguration<IndustrySubCategory>
{
    public static IndustrySubCategory[] SeedData =>
    [
        new()
        {
            Id = "HauXa9RY9QSr8a54ByFwj",
            Name = "Building Materials",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Construction").Id
        },
        new()
        {
            Id = "hJNIh-Im_f7clkYZmZBRE",
            Name = "Civil Engineering",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Construction").Id
        },
        new()
        {
            Id = "zQRObEMkSHN8QOtTZBwkJ",
            Name = "Construction",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Construction").Id
        },
        new()
        {
            Id = "ih37Ze5aaOGfG7TYXUsbv",
            Name = "Architecture & Planning",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Design").Id
        },
        new()
        {
            Id = "HEN-ijto0K1Sf_OciHDIq",
            Name = "Design",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Design").Id
        },
        new()
        {
            Id = "btsPkeHzW4sNny11BSvof",
            Name = "Graphic Design",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Design").Id
        },
        new()
        {
            Id = "Anp0oogRUzx-UZ1clwGAa",
            Name = "Accounting",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Corporate Services").Id
        },
        new()
        {
            Id = "MXUwHkgay_UZXX2AzY1FU",
            Name = "Business Supplies & Equipment",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Corporate Services").Id
        },
        new()
        {
            Id = "tkF5aFhlRnPPhRsodehd9",
            Name = "Environmental Services",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Corporate Services").Id
        },
        new()
        {
            Id = "68AhtustNiXT8H107NbXo",
            Name = "Events Services",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Corporate Services").Id
        },
        new()
        {
            Id = "K3SqEjUcLSaj-OASf-Vfn",
            Name = "Executive Office",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Corporate Services").Id
        },
        new()
        {
            Id = "P5qA_npwKevf0IuQQ-o8V",
            Name = "Facilities Services",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Corporate Services").Id
        },
        new()
        {
            Id = "PwffwJ0sXibSyQ4oXdBk4",
            Name = "Human Resources",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Corporate Services").Id
        },
        new()
        {
            Id = "oGHZiJyHGGZRlNz9ooLj_",
            Name = "Information Services",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Corporate Services").Id
        },
        new()
        {
            Id = "_GJ2pKqtFHpFFWeq5uP7A",
            Name = "Management Consulting",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Corporate Services").Id
        },
        new()
        {
            Id = "Wcxg0azu7qwIOnESWfKD6",
            Name = "Outsourcing/Offshoring",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Corporate Services").Id
        },
        new()
        {
            Id = "XoecAXYxdwsdOQyH44as-",
            Name = "Professional Training & Coaching",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Corporate Services").Id
        },
        new()
        {
            Id = "hzE99r9OYpChAm-J-I95R",
            Name = "Security & Investigations",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Corporate Services").Id
        },
        new()
        {
            Id = "fMq27yPPSWdyt56OHGARE",
            Name = "Staffing & Recruiting",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Corporate Services").Id
        },
        new()
        {
            Id = "9SqM8gfgtFgKDpUr1cNco",
            Name = "Retail",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Retail").Id
        },
        new()
        {
            Id = "Kzq6AwGAsKU2tEky6V6Ym",
            Name = "Supermarkets",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Retail").Id
        },
        new()
        {
            Id = "jNfz-hPjT4aCuU6Ul0uee",
            Name = "Wholesale",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Retail").Id
        },
        new()
        {
            Id = "kmCWCF7gBidij5s3qIBFa",
            Name = "Mining & Metals",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Energy & Mining").Id
        },
        new()
        {
            Id = "wuaS-83ScePOPqZDXbSWd",
            Name = "Oil & Energy",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Energy & Mining").Id
        },
        new()
        {
            Id = "ideEWzCBrRa2lyrKp2lAb",
            Name = "Utilities",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Energy & Mining").Id
        },
        new()
        {
            Id = "P_Qo9uJlw5ERFBfAxI5PV",
            Name = "Automotive",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Manufacturing").Id
        },
        new()
        {
            Id = "S4hTeNXKIRL84bRRzli1k",
            Name = "Aviation & Aerospace",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Manufacturing").Id
        },
        new()
        {
            Id = "nMV4PpF25mWpYwN-tSIB0",
            Name = "Chemicals",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Manufacturing").Id
        },
        new()
        {
            Id = "PGAC2vXJHtktpqxfiw5TQ",
            Name = "Defense & Space",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Manufacturing").Id
        },
        new()
        {
            Id = "KfojPdlEXy5bhjhv9Buti",
            Name = "Electrical & Electronic Manufacturing",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Manufacturing").Id
        },
        new()
        {
            Id = "rC3xY9zAu8f6RCUPSKVjD",
            Name = "Food Production",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Manufacturing").Id
        },
        new()
        {
            Id = "gojKcjl4sTRVokd6Fwm5Q",
            Name = "Glass, Ceramics & Concrete",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Manufacturing").Id
        },
        new()
        {
            Id = "Fl8iL9jTkTcDLniwG8PjM",
            Name = "Industrial Automation",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Manufacturing").Id
        },
        new()
        {
            Id = "5_xJDrAIJrMsS8SfvyJ9z",
            Name = "Machinery",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Manufacturing").Id
        },
        new()
        {
            Id = "XOKkzryiG28mrlGfF5_H4",
            Name = "Mechanical or Industrial Engineering",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Manufacturing").Id
        },
        new()
        {
            Id = "tkzVx19aJOia3kGSyaug0",
            Name = "Packaging & Containers",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Manufacturing").Id
        },
        new()
        {
            Id = "qh8mwzmfi1NuEjGc8z5S0",
            Name = "Paper & Forest Products",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Manufacturing").Id
        },
        new()
        {
            Id = "8sdTPVkp6YM4gwIDd6yTL",
            Name = "Plastics",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Manufacturing").Id
        },
        new()
        {
            Id = "d6K3MC3hA_zZXsGRMyHSM",
            Name = "Railroad Manufacture",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Manufacturing").Id
        },
        new()
        {
            Id = "XaZJbtarjgHa9aJfGOllq",
            Name = "Renewables & Environment",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Manufacturing").Id
        },
        new()
        {
            Id = "mZE0AvfFQrQAPbExKJTZK",
            Name = "Shipbuilding",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Manufacturing").Id
        },
        new()
        {
            Id = "mrVhAaI2m_3ZA7xnddJFt",
            Name = "Textiles",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Manufacturing").Id
        },
        new()
        {
            Id = "hMrZmeoHvKaOy0ufUZa35",
            Name = "Banking",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Finance").Id
        },
        new()
        {
            Id = "Z7swakPNl1_vEUHT-uip3",
            Name = "Capital Markets",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Finance").Id
        },
        new()
        {
            Id = "2wvDDgBTMDs5pDVlbS1OX",
            Name = "Financial Services",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Finance").Id
        },
        new()
        {
            Id = "EIJHhNw_wpIVL3pA1OOZr",
            Name = "Insurance",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Finance").Id
        },
        new()
        {
            Id = "1vbfjkAaHk0w6svYoDBjd",
            Name = "Investment Banking",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Finance").Id
        },
        new()
        {
            Id = "GbvxgUqTZIi_eJLzHk101",
            Name = "Investment Management",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Finance").Id
        },
        new()
        {
            Id = "WmZvpAjtGwZ_UBAzXE8qR",
            Name = "Venture Capital & Private Equity",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Finance").Id
        },
        new()
        {
            Id = "6FC0NBRxizMTAvhm-huwF",
            Name = "Airlines/Aviation",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Recreation & Travel").Id
        },
        new()
        {
            Id = "K_rfAbaRdODY0FzSi4eCP",
            Name = "Gambling & Casinos",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Recreation & Travel").Id
        },
        new()
        {
            Id = "cE5QYVgV7Rug2iEmhPSLp",
            Name = "Hospitality",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Recreation & Travel").Id
        },
        new()
        {
            Id = "LnRhNtxZehp9pPTaHrVcx",
            Name = "Leisure, Travel & Tourism",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Recreation & Travel").Id
        },
        new()
        {
            Id = "eTmgDsQHooF8yfjQibuZx",
            Name = "Restaurants",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Recreation & Travel").Id
        },
        new()
        {
            Id = "UxWZXcAvWUEMrwqvlLKcR",
            Name = "Recreational Facilities & Services",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Recreation & Travel").Id
        },
        new()
        {
            Id = "H9-dThKHpcKEPj_ncmau8",
            Name = "Sports",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Recreation & Travel").Id
        },
        new()
        {
            Id = "WTd59z5le2R3ljmwoSOMo",
            Name = "Arts & Crafts",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Arts").Id
        },
        new()
        {
            Id = "MbTSEkTyZa4sCNPlhzybY",
            Name = "Fine Art",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Arts").Id
        },
        new()
        {
            Id = "EBrEnqmLfty7fdbbUCe4f",
            Name = "Performing Arts",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Arts").Id
        },
        new()
        {
            Id = "TKY-GyWAwwhNKtSHVgfjS",
            Name = "Photography",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Arts").Id
        },
        new()
        {
            Id = "xGRNZeN51DBhyNcvIDpmp",
            Name = "Biotechnology",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Health Care").Id
        },
        new()
        {
            Id = "vggS9I-URCNvKv5lDV4Qv",
            Name = "Hospital & Health Care",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Health Care").Id
        },
        new()
        {
            Id = "Qc81ixxiDmrK2nOdrAmmL",
            Name = "Medical Device",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Health Care").Id
        },
        new()
        {
            Id = "yXJjHKfGxQkrrX8IIQBoX",
            Name = "Medical Practice",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Health Care").Id
        },
        new()
        {
            Id = "LG7xy63xPAdigL2noaJ3Q",
            Name = "Mental Health Care",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Health Care").Id
        },
        new()
        {
            Id = "ETHXo2XAWdvAVeNraKr7o",
            Name = "Pharmaceuticals",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Health Care").Id
        },
        new()
        {
            Id = "kc6a_bX7NdI8XLPo9O_OQ",
            Name = "Veterinary",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Health Care").Id
        },
        new()
        {
            Id = "4fHASimX1R3Kb-soGZcTn",
            Name = "Computer Hardware",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Hardware & Networking").Id
        },
        new()
        {
            Id = "2KfFED1vb65J4w0HIAH4o",
            Name = "Computer Networking",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Hardware & Networking").Id
        },
        new()
        {
            Id = "3ALho9Wdd4nPstkcPCaqo",
            Name = "Nanotechnologie",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Hardware & Networking").Id
        },
        new()
        {
            Id = "OhG4N-hdXRWFsnO3mQQ_K",
            Name = "Semiconductors",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Hardware & Networking").Id
        },
        new()
        {
            Id = "mHMMNmKLOEtOvMHM3Vv53",
            Name = "Telecommunications",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Hardware & Networking").Id
        },
        new()
        {
            Id = "nnUTfoWcex-OQST0GSBKx",
            Name = "Wireless",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Hardware & Networking").Id
        },
        new()
        {
            Id = "PDzsPTIM6XZ6wG04mfB1o",
            Name = "Commercial Real Estate",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Real Estate").Id
        },
        new()
        {
            Id = "tOLdxfJbbyZseUsruul4F",
            Name = "Real Estate",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Real Estate").Id
        },
        new()
        {
            Id = "NwRiGUxTpWjBD_QLTmgb1",
            Name = "Alternative Dispute Resolution",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Legal").Id
        },
        new()
        {
            Id = "9lwcTkiRLUx1-yK8CsRc3",
            Name = "Law Practice",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Legal").Id
        },
        new()
        {
            Id = "LIY449onOi0tKM7y5Lf2E",
            Name = "Legal Services",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Legal").Id
        },
        new()
        {
            Id = "DXvW70szUeBs2Qt_FUTyn",
            Name = "Apparel & Fashion",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Consumer Goods").Id
        },
        new()
        {
            Id = "HkVmjXmMbNK5rW_yErWrB",
            Name = "Consumer Electronics",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Consumer Goods").Id
        },
        new()
        {
            Id = "Cv4P4IfMXEFDds1WILHMf",
            Name = "Consumer Goods",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Consumer Goods").Id
        },
        new()
        {
            Id = "_TTaYtfsEUmytV_9n2PQy",
            Name = "Consumer Services",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Consumer Goods").Id
        },
        new()
        {
            Id = "4NQjYM6jdfnKVRwz5vJzK",
            Name = "Cosmetics",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Consumer Goods").Id
        },
        new()
        {
            Id = "WYpGJyTVuuM92igAmspbE",
            Name = "Food & Beverages",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Consumer Goods").Id
        },
        new()
        {
            Id = "DoZAnGTFeGb2lrW--1pJi",
            Name = "Furniture",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Consumer Goods").Id
        },
        new()
        {
            Id = "0LvvVx4gPaRD73p9dAKA6",
            Name = "Luxury Goods & Jewelry",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Consumer Goods").Id
        },
        new()
        {
            Id = "Xoox6447RFMKGcnSQRyCM",
            Name = "Sporting Goods",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Consumer Goods").Id
        },
        new()
        {
            Id = "cyxCXes3Pi4QN_DoTq7QQ",
            Name = "Tobacco",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Consumer Goods").Id
        },
        new()
        {
            Id = "PmYm5CdXqxfmNJU9KYpQK",
            Name = "Wine and Spirits",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Consumer Goods").Id
        },
        new()
        {
            Id = "pHYIZBESp80D3ZV4Sn7rV",
            Name = "Dairy",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Agriculture").Id
        },
        new()
        {
            Id = "xVSkoLNA2s6T9Wo1uFD6R",
            Name = "Farming",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Agriculture").Id
        },
        new()
        {
            Id = "1OQUodxh6dXzBAzyhE_In",
            Name = "Fishery",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Agriculture").Id
        },
        new()
        {
            Id = "HDkZbXrHsEz9108zjzU3N",
            Name = "Ranching",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Agriculture").Id
        },
        new()
        {
            Id = "v36gThLN0qbHByjyu_Gl_",
            Name = "Market Research",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Media & Communications").Id
        },
        new()
        {
            Id = "QZvLjlV-l-WUMAsoAVBj-",
            Name = "Marketing & Advertising",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Media & Communications").Id
        },
        new()
        {
            Id = "zfC86Kgwc8f9fzP5latH9",
            Name = "Newspapers",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Media & Communications").Id
        },
        new()
        {
            Id = "psDvbeMLWDuDiUG6RLtQm",
            Name = "Online Media",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Media & Communications").Id
        },
        new()
        {
            Id = "sr2AuEYun_pZUWZI7gQcz",
            Name = "Printing",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Media & Communications").Id
        },
        new()
        {
            Id = "RgKCDwUMXB5cLaNI5YZ9_",
            Name = "Public Relations & Communications",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Media & Communications").Id
        },
        new()
        {
            Id = "dkD2WRvcb33CAjsB35l1r",
            Name = "Publishing",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Media & Communications").Id
        },
        new()
        {
            Id = "sk3VI48ZM0PXC5yKnB91Z",
            Name = "Translation & Localization",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Media & Communications").Id
        },
        new()
        {
            Id = "Rc0rGZQCEu39Bw6WdutRL",
            Name = "Writing & Editing",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Media & Communications").Id
        },
        new()
        {
            Id = "Gi_MSree2JJPgxibLvwS-",
            Name = "Civic & Social Organization",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Nonprofit").Id
        },
        new()
        {
            Id = "esqKnQGv3x5aqWniYO6qn",
            Name = "Fundraising",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Nonprofit").Id
        },
        new()
        {
            Id = "8y4ShEofzG0tIDWcDFRsL",
            Name = "Individual & Family Services",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Nonprofit").Id
        },
        new()
        {
            Id = "4b5K73V1j6E0pXpTT6EF2",
            Name = "International Trade & Development",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Nonprofit").Id
        },
        new()
        {
            Id = "SmwTdBTItdXOsYU1tIBhL",
            Name = "Libraries",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Nonprofit").Id
        },
        new()
        {
            Id = "3QLolUIA1CaLD1KV01QmU",
            Name = "Museums & Institutions",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Nonprofit").Id
        },
        new()
        {
            Id = "vgBXbrQLaCmsOQVfy68ob",
            Name = "Non-Profit Organization Management",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Nonprofit").Id
        },
        new()
        {
            Id = "7HyAeDaXYrv24Mfsr9Vs8",
            Name = "Philanthropy",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Nonprofit").Id
        },
        new()
        {
            Id = "WF-XxhBCdD2UN1tZXzWXd",
            Name = "Program Development",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Nonprofit").Id
        },
        new()
        {
            Id = "uIE9NifoiClxefpceV_X2",
            Name = "Religious Institutions",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Nonprofit").Id
        },
        new()
        {
            Id = "KplQglXodzYl59gK_epnf",
            Name = "Think Tanks",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Nonprofit").Id
        },
        new()
        {
            Id = "6fV2OxmlTsfoOVVJ9KFfk",
            Name = "Computer & Network Security",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Software & IT Services").Id
        },
        new()
        {
            Id = "DRD09Q2dKJGDJSp_0_64W",
            Name = "Computer Software",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Software & IT Services").Id
        },
        new()
        {
            Id = "LUyonPGmj7DGOXCnvRKn_",
            Name = "Information Technology & Services",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Software & IT Services").Id
        },
        new()
        {
            Id = "Y-EXiW8yWEtnM6MdMq3MD",
            Name = "Internet",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Software & IT Services").Id
        },
        new()
        {
            Id = "jDUPoojddO2IZNNk7HLn_",
            Name = "Import & Export",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Transportation & Logistics").Id
        },
        new()
        {
            Id = "UugkhQXKrqMj0btvXi7S8",
            Name = "Logistics & Supply Chain",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Transportation & Logistics").Id
        },
        new()
        {
            Id = "8Sbp7Tb4ktevjRWWruzye",
            Name = "Maritime",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Transportation & Logistics").Id
        },
        new()
        {
            Id = "9hvvmVHeIYptCbkUhj0jH",
            Name = "Package/Freight Delivery",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Transportation & Logistics").Id
        },
        new()
        {
            Id = "oCfNjmm0Q5YCrR_qnXhfv",
            Name = "Transportation/Trucking/Railroad",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Transportation & Logistics").Id
        },
        new()
        {
            Id = "4mEHLACzvNFW_GUDtRxjy",
            Name = "Warehousing",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Transportation & Logistics").Id
        },
        new()
        {
            Id = "pGmd11WrkICGA8ZbHSJXZ",
            Name = "Animation",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Entertainment").Id
        },
        new()
        {
            Id = "xTZpJWTpdotpEglrHqPvd",
            Name = "Broadcast Media",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Entertainment").Id
        },
        new()
        {
            Id = "PLW5y9Q2M5jZxxzYsZ1zL",
            Name = "Computer Games",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Entertainment").Id
        },
        new()
        {
            Id = "dxmzLuPtXJqUGc4rRYl_8",
            Name = "Entertainment",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Entertainment").Id
        },
        new()
        {
            Id = "lrjZuf6_7u2jIgNHNIYEx",
            Name = "Media Production",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Entertainment").Id
        },
        new()
        {
            Id = "_tUue6Gl-953pMKTqN7TI",
            Name = "Mobile Games",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Entertainment").Id
        },
        new()
        {
            Id = "BsRUgUyxfncNkCq4wpLJj",
            Name = "Motion Pictures & Film",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Entertainment").Id
        },
        new()
        {
            Id = "dwWQZ-qlYhqDloUB0yQhW",
            Name = "Music",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Entertainment").Id
        },
        new()
        {
            Id = "N9NMqIS5yQTzU5GYDLGaS",
            Name = "Alternative Medicine",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Wellness & Fitness").Id
        },
        new()
        {
            Id = "bHXQAM_dBX_jf3YaPVtzn",
            Name = "Health, Wellness & Fitness",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Wellness & Fitness").Id
        },
        new()
        {
            Id = "CAtDPbb6rU1Vz2gPAhPlv",
            Name = "Law Enforcement",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Public Safety").Id
        },
        new()
        {
            Id = "jCLq3vnUK-nibBg1JOK_O",
            Name = "Military",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Public Safety").Id
        },
        new()
        {
            Id = "ngm0upQBvO_Vn-4gsEEJx",
            Name = "Public Safety",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Public Safety").Id
        },
        new()
        {
            Id = "2Dn5XQlDSvNIuPcNz1Z2_",
            Name = "Government Administration",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Public Administration").Id
        },
        new()
        {
            Id = "R6UJ7bC5s5RET284J7fTI",
            Name = "Government Relations",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Public Administration").Id
        },
        new()
        {
            Id = "pPypbwxYDCO_vayD_m1vR",
            Name = "International Affairs",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Public Administration").Id
        },
        new()
        {
            Id = "MM_23S9wXuT1XN0hvCpqD",
            Name = "Judiciary",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Public Administration").Id
        },
        new()
        {
            Id = "PFk8EMo95ki7CBxEFOhgp",
            Name = "Legislative Office",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Public Administration").Id
        },
        new()
        {
            Id = "6AUKesJxOibCNaXiVxejR",
            Name = "Political Organization",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Public Administration").Id
        },
        new()
        {
            Id = "RhArj4cCu9IkPqCmTXpnL",
            Name = "Public Policy",
            IndustryMainCategoryId = IndustryMainCategoryConfiguration.SeedData.Single(item => item.Name == "Public Administration").Id
        }
    ];

    public void Configure(EntityTypeBuilder<IndustrySubCategory> builder)
    {
        builder.ConfigureEntityBaseWithDeleted();

        builder.Property(item => item.Name).HasMaxLength(Constants.MaxOrganizationIndustrySubCategoryNameLength);

        builder
            .HasOne(item => item.IndustryMainCategory)
            .WithMany(item => item.IndustrySubCategories)
            .HasForeignKey(item => item.IndustryMainCategoryId);

        builder.HasData(SeedData.Select(item =>
        {
            item.CreatedAt = new DateTimeOffset(new DateTime(2024, 5, 1, 0, 0, 0, 0, DateTimeKind.Utc), new TimeSpan(0, 0, 0, 0, 0));
            return item;
        }));

        builder.HasIndex(item => item.Name);
    }
}
