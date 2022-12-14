// <auto-generated />
using System;
using BeerSender.API.Read_models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BeerSender.API.Migrations.ReadcontextMigrations
{
    [DbContext(typeof(Read_context))]
    [Migration("20221118170323_CreateReadDb")]
    partial class CreateReadDb
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("BeerSender.API.Read_models.Package_beer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Beer_name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Brewery")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("PackageId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("Package_beers");
                });

            modelBuilder.Entity("BeerSender.API.Read_models.Projection_checkpoint", b =>
                {
                    b.Property<string>("Projection_name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("Event_id")
                        .HasColumnType("int");

                    b.HasKey("Projection_name");

                    b.ToTable("Projection_checkpoints");
                });
#pragma warning restore 612, 618
        }
    }
}
