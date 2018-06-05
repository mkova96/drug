using DrugData.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DrugData
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {

                if (context.SideEffect.Any())
                {
                    return;
                }
                var sds = new[]{
                    new SideEffect{SideEffectName="Glavobolja"},
                    new SideEffect{SideEffectName="Povišena temperatura"},
                    new SideEffect{SideEffectName="Znojenje"},
                    new SideEffect{SideEffectName="Umor"},
                    new SideEffect{SideEffectName="Anksioznost"},
                    new SideEffect{SideEffectName="Nervoza"},
                    new SideEffect{SideEffectName="Mučnina"},
                    new SideEffect{SideEffectName="Proljev"},
                    new SideEffect{SideEffectName="Drhtanje"},



                };
                context.SideEffect.AddRange(sds);
                context.SaveChanges();

                ////////////////////////////////////////////////

                if (context.Disease.Any())
                {
                    return;
                }
                var ds = new[]{
                    new Disease{DiseaseName="Konjunktivitis",ICD="H10"},
                    new Disease{DiseaseName="Keratitis",ICD="H16"},
                    new Disease{DiseaseName="Gluakom",ICD="H40"},
                    new Disease{DiseaseName="Upala vanjskog uha",ICD="H60"},
                    new Disease{DiseaseName="Antaraks",ICD="A22"},
                    new Disease{DiseaseName="Listerioza",ICD="A32"},





                };
                context.Disease.AddRange(ds);
                context.SaveChanges();

                ////////////////////////////////////////////////

                if (context.Manufacturer.Any())
                {
                    return;
                }
                var labels = new[]{
                    new Manufacturer{ManufacturerName="Pliva", Drugs=new List<Medication>()},
                    new Manufacturer{ManufacturerName="Belupo", Drugs=new List<Medication>()},
                    new Manufacturer{ManufacturerName="Sandoz", Drugs=new List<Medication>()},
                };
                context.Manufacturer.AddRange(labels);
                context.SaveChanges();

                //////////////////////////////////////////////////

                if (context.Currency.Any())
                {
                    return;
                }
                var currencies = new[]{
                    new Currency{CurrencyName="HRK"}
                };
                context.Currency.AddRange(currencies);
                context.SaveChanges();

                //////////////////////////////////////////////////

                if (context.Measure.Any())
                {
                    return;
                }
                var measures = new[]{
                    new Measure{MeasureName="mg"},
                    new Measure{MeasureName="g"},
                    new Measure{MeasureName="kg"},
                    new Measure{MeasureName="ml"},
                    new Measure{MeasureName="l"},

                };
                context.Measure.AddRange(measures);
                context.SaveChanges();

                //////////////////////////////////////////////////////

                if (context.Country.Any())
                {
                    return;
                }
                var countries = new[]{
                    new Country { CountryName = "Hrvatska" ,Cities=new List<City>()},
                 };
                context.Country.AddRange(countries);
                context.SaveChanges();

                /////////////////////////////////////////////////////////

                if (context.City.Any())
                {
                    return;
                }
                var cities = new[]
                {
                   new City {PostCode=10000,CityName="Zagreb",Country=countries[0]},
                   new City {PostCode=21000,CityName="Split",Country=countries[0]},
                   new City {PostCode=51000,CityName="Rijeka",Country=countries[0]},
                   new City {PostCode=31000,CityName="Osijek",Country=countries[0]},
                   new City {PostCode=23000,CityName="Zadar",Country=countries[0]},
                   new City {PostCode=22000,CityName="Šibenik",Country=countries[0]},
                   new City {PostCode=20000,CityName="Dubrovnik",Country=countries[0]},
                   new City {PostCode=52210,CityName="Rovinj",Country=countries[0]},
                   new City {PostCode=43000,CityName="Bjelovar",Country=countries[0]},
                   new City {PostCode=35000,CityName="Slavonski Brod",Country=countries[0]},
               };
                context.City.AddRange(cities);
                context.SaveChanges();

                countries[0].Cities.Add(cities[0]);
                countries[0].Cities.Add(cities[1]);
                countries[0].Cities.Add(cities[2]);
                countries[0].Cities.Add(cities[3]);
                countries[0].Cities.Add(cities[4]);
                countries[0].Cities.Add(cities[5]);
                countries[0].Cities.Add(cities[6]);
                countries[0].Cities.Add(cities[7]);
                countries[0].Cities.Add(cities[8]);
                countries[0].Cities.Add(cities[9]);

                context.SaveChanges();


            }
        }

    }
}
