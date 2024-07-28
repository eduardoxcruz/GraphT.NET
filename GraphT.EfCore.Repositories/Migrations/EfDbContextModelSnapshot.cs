﻿// <auto-generated />
using System;
using GraphT.EfCore.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GraphT.EfCore.Repositories.Migrations
{
    [DbContext(typeof(EfDbContext))]
    partial class EfDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("GraphT.EfCore.Repositories.Models.TodoTaskStream", b =>
                {
                    b.Property<Guid>("UpstreamTaskId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("DownstreamTaskId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UpstreamTaskId", "DownstreamTaskId");

                    b.HasIndex("DownstreamTaskId");

                    b.ToTable("TaskStreams");
                });

            modelBuilder.Entity("GraphT.Model.Entities.TodoTask", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("Id")
                        .HasColumnOrder(0);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnOrder(1);

                    b.Property<int>("Status")
                        .HasColumnType("int")
                        .HasColumnOrder(4);

                    b.Property<string>("TaskType")
                        .IsRequired()
                        .HasMaxLength(13)
                        .HasColumnType("nvarchar(13)");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.ToTable("TodoTasks");

                    b.HasDiscriminator<string>("TaskType").HasValue("TodoTask");

                    b.UseTphMappingStrategy();
                });

            modelBuilder.Entity("GraphT.Model.Aggregates.TaskAggregate", b =>
                {
                    b.HasBaseType("GraphT.Model.Entities.TodoTask");

                    b.Property<int>("Complexity")
                        .HasColumnType("int")
                        .HasColumnOrder(5);

                    b.Property<bool>("IsFun")
                        .HasColumnType("bit")
                        .HasColumnOrder(2);

                    b.Property<bool>("IsProductive")
                        .HasColumnType("bit")
                        .HasColumnOrder(3);

                    b.Property<int>("Priority")
                        .HasColumnType("int")
                        .HasColumnOrder(6);

                    b.Property<int>("Relevance")
                        .HasColumnType("int")
                        .HasColumnOrder(7);

                    b.HasDiscriminator().HasValue("TaskAggregate");
                });

            modelBuilder.Entity("GraphT.EfCore.Repositories.Models.TodoTaskStream", b =>
                {
                    b.HasOne("GraphT.Model.Entities.TodoTask", "Downstream")
                        .WithMany()
                        .HasForeignKey("DownstreamTaskId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("GraphT.Model.Entities.TodoTask", "Upstream")
                        .WithMany()
                        .HasForeignKey("UpstreamTaskId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Downstream");

                    b.Navigation("Upstream");
                });
#pragma warning restore 612, 618
        }
    }
}
