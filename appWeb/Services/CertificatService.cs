using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using appWeb.Models;
using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace appWeb.Services
{
    public class CertificatService
    {
        private readonly IWebHostEnvironment _env;

        public CertificatService(IWebHostEnvironment env)
        {
            _env = env;
            // Active le mode débogage QuestPDF (inutile en production)
            QuestPDF.Settings.EnableDebugging = true;
        }

        [Obsolete]
        public byte[] GenererCertificat(Stagiaire stagiaire, Formation formation, Encadrant encadrant, Admin admin)
        {
            string logoRelativePath = admin.LogoPath;

            if (string.IsNullOrEmpty(logoRelativePath))
            {
                logoRelativePath = "images/logo.jpg"; // fallback
            }
            else
            {
                // Enlever le slash initial s'il existe
                if (logoRelativePath.StartsWith("/") || logoRelativePath.StartsWith("\\"))
                {
                    logoRelativePath = logoRelativePath.TrimStart('/', '\\');
                }
            }

            string logoPath = Path.Combine(_env.WebRootPath, logoRelativePath);
            string iconCertPath = Path.Combine(_env.WebRootPath, "images", "Cer.png");
            string backgroundPath = Path.Combine(_env.WebRootPath, "images", "back.jpg");

            int nbMois = ((formation.DateFin.Year - formation.DateDebut.Year) * 12) + formation.DateFin.Month - formation.DateDebut.Month;
            if (formation.DateFin.Day < formation.DateDebut.Day)
                nbMois--;

            string dureeStage = nbMois <= 1 ? "1 mois" : $"{nbMois} mois";

            string phraseDuree;

            if (nbMois <= 0)
                phraseDuree = " inférieure au mois";
            else if (nbMois == 1)
                phraseDuree = " d'un mois";
            else if (nbMois == 12)
                phraseDuree = " d’un an";
            else
                phraseDuree = $" de {nbMois} mois";

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(30);
                    page.PageColor("#fefefe");
                    page.DefaultTextStyle(x => x.FontFamily("Times New Roman").FontSize(18));

                    // Fond optionnel
                    if (File.Exists(backgroundPath))
                    {
                        page.Background().Image(backgroundPath, ImageScaling.FitArea);
                    }

                    // Header
                    page.Header().PaddingBottom(10).Row(row =>
                    {
                        row.ConstantColumn(100).Element(container =>
                        {
                            if (File.Exists(logoPath))
                                container.Image(logoPath, ImageScaling.FitArea);
                        });

                        row.RelativeColumn().AlignCenter().Column(col =>
                        {
                            col.Spacing(20);
                            col.Item().PaddingBottom(10).AlignCenter().Stack(stack =>
                            {


                                stack.Item().Text("               ")
                                    .FontSize(20)
                                    .Bold()
                                    .FontColor("#1a73e8");
                                    
                                    
                                stack.Item().Text("***Certificat de Stage***")
                                    .FontSize(25)
                                    .Bold()
                                    .FontColor("#1a73e8")
                                    .Underline()
                                    .FontFamily("Georgia");

                                stack.Item().Text("               ")
                                    .FontSize(20)
                                    .Bold()
                                    .FontColor("#1a73e8");

                                stack.Item().Text("Encadrement Professionnel des Stagiaires")
                                    .FontSize(18)
                                    .Italic()
                                    .FontColor("#555555")
                                    .FontFamily("Times New Roman");

                                stack.Item().LineHorizontal(2).LineColor("#1a73e8");
                            });
                        });

                        row.ConstantColumn(80).Element(container =>
                        {
                            if (File.Exists(iconCertPath))
                                container.Image(iconCertPath, ImageScaling.FitArea);
                        });
                    });

                    // Corps du certificat
                    page.Content().PaddingTop(20).Column(col =>
                    {
                        col.Spacing(20);

                        col.Item().AlignCenter().Text("Ce certificat est délivré à")
                            .FontSize(20).Italic().FontColor("#555");

                        col.Item().AlignCenter().Text($"{stagiaire.Nom} {stagiaire.Prenom}")
                            .FontSize(30).Bold().FontColor("#000000");

                        col.Item().AlignCenter().Text("Pour avoir accompli avec succès un stage au sein de notre établissement dans le cadre de la formation :")
                            .FontSize(18).Italic().FontColor("#333");

                        col.Item().AlignCenter().Text(formation.Titre)
                            .FontSize(24).Bold().FontColor("#000000");

                        col.Item().AlignCenter().Text($"Période du stage : du {formation.DateDebut:dd/MM/yyyy} au {formation.DateFin:dd/MM/yyyy}")
                            .FontSize(16).FontColor("#444");

                        col.Item().PaddingVertical(15).Text(text =>
                        {
                            text.DefaultTextStyle(x => x.FontSize(14).FontColor("#555"));

                            text.Line(" ");
                            text.Line("     Ce certificat atteste que le stagiaire a participé activement aux tâches confiées a fait preuve");
                            text.Line($"     de sérieux et d’engagement tout au long de son stage d'une durée {phraseDuree}.");
                           
                            text.Line("     il a acquis des compétences significatives dans son domaine de stage et constitue une  ");
                            text.Line("     reconnaissance de son investissement professionnel et de sa progression.");
                        });

                        col.Item().AlignCenter().Text($"Fait à {admin.Ville}, le {DateTime.Now:dd/MM/yyyy}")
                            .FontSize(14).Italic().FontColor("#777");

                        // Signatures
                        col.Item().PaddingTop(30).Row(row =>
                        {
                            row.RelativeColumn().AlignCenter().Column(signatureCol =>
                            {
                                signatureCol.Item().Text("________________________").FontSize(14);
                                signatureCol.Item().Text("Encadrant de stagiaire").FontSize(14);
                                signatureCol.Item().Text($"{encadrant.Nom} {encadrant.Prenom}").FontSize(14).Italic().FontColor("#444");
                            });

                            row.RelativeColumn().AlignCenter().Column(signatureCol =>
                            {
                                signatureCol.Item().Text("________________________").FontSize(14);
                                signatureCol.Item().Text("Cachet de l’établissement").FontSize(14);
                                signatureCol.Item().Text($"{admin.Entreprise}").FontSize(14).Italic().FontColor("#444");

                            });
                        });
                    });

                    // Footer
                    page.Footer().PaddingTop(10).PaddingBottom(50).AlignCenter()
                        .Text("© 2025 - Stage.ma")
                        .FontSize(10).FontColor(Colors.Grey.Medium);
                });
            });
            return document.GeneratePdf();
        }

    }
}

