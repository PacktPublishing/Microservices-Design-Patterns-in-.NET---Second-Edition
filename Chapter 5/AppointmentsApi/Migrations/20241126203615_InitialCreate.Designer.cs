﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AppointmentsApi.Migrations
{
    [DbContext(typeof(AppointmentContext))]
    [Migration("20241126203615_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0");

            modelBuilder.Entity("AppointmentsApi.Models.Appointment", b =>
                {
                    b.Property<Guid>("AppointmentId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("DoctorId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("PatientId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Purpose")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("AppointmentId");

                    b.ToTable("Appointments");
                });

            modelBuilder.Entity("AppointmentsApi.Models.Appointment", b =>
                {
                    b.OwnsOne("AppointmentsApi.Models.Location", "Location", b1 =>
                        {
                            b1.Property<Guid>("AppointmentId")
                                .HasColumnType("TEXT");

                            b1.Property<string>("Building")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<string>("RoomNumber")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.HasKey("AppointmentId");

                            b1.ToTable("Appointments");

                            b1.WithOwner()
                                .HasForeignKey("AppointmentId");
                        });

                    b.OwnsOne("AppointmentsApi.Models.TimeSlot", "Slot", b1 =>
                        {
                            b1.Property<Guid>("AppointmentId")
                                .HasColumnType("TEXT");

                            b1.Property<DateTime>("End")
                                .HasColumnType("TEXT");

                            b1.Property<DateTime>("Start")
                                .HasColumnType("TEXT");

                            b1.HasKey("AppointmentId");

                            b1.ToTable("Appointments");

                            b1.WithOwner()
                                .HasForeignKey("AppointmentId");
                        });

                    b.Navigation("Location")
                        .IsRequired();

                    b.Navigation("Slot")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
