﻿// <auto-generated />
using ChatApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ChatApp.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20230614155621_AddedChatTable")]
    partial class AddedChatTable
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("ChatApp.Models.Chat", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ChannelId")
                        .HasColumnType("text")
                        .HasColumnName("channel_id");

                    b.Property<string>("Message")
                        .HasColumnType("text")
                        .HasColumnName("message");

                    b.Property<short>("MessageType")
                        .HasColumnType("smallint")
                        .HasColumnName("message_type");

                    b.Property<bool>("Seen")
                        .HasColumnType("boolean")
                        .HasColumnName("seen");

                    b.Property<string>("UserId")
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_chats");

                    b.ToTable("chats", (string)null);
                });

            modelBuilder.Entity("ChatApp.Models.Group", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("DpUrl")
                        .HasColumnType("text")
                        .HasColumnName("dp_url");

                    b.Property<string>("Name")
                        .HasColumnType("text")
                        .HasColumnName("name");

                    b.HasKey("Id")
                        .HasName("pk_groups");

                    b.ToTable("groups", (string)null);
                });

            modelBuilder.Entity("ChatApp.Models.GroupInfo", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("GroupId")
                        .HasColumnType("text")
                        .HasColumnName("group_id");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("boolean")
                        .HasColumnName("is_admin");

                    b.Property<string>("UserId")
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_group_infos");

                    b.HasIndex("GroupId")
                        .HasDatabaseName("ix_group_infos_group_id");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_group_infos_user_id");

                    b.ToTable("group_infos", (string)null);
                });

            modelBuilder.Entity("ChatApp.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("DpUrl")
                        .HasColumnType("text")
                        .HasColumnName("dp_url");

                    b.Property<string>("Email")
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<string>("Password")
                        .HasColumnType("text")
                        .HasColumnName("password");

                    b.Property<string>("UserName")
                        .HasColumnType("text")
                        .HasColumnName("user_name");

                    b.HasKey("Id")
                        .HasName("pk_users");

                    b.ToTable("users", (string)null);
                });

            modelBuilder.Entity("ChatApp.Models.UserActiveChat", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text")
                        .HasColumnName("id");

                    b.Property<string>("ChannelId")
                        .HasColumnType("text")
                        .HasColumnName("channel_id");

                    b.Property<string>("UserId")
                        .HasColumnType("text")
                        .HasColumnName("user_id");

                    b.HasKey("Id")
                        .HasName("pk_user_active_chats");

                    b.HasIndex("UserId")
                        .HasDatabaseName("ix_user_active_chats_user_id");

                    b.ToTable("user_active_chats", (string)null);
                });

            modelBuilder.Entity("ChatApp.Models.GroupInfo", b =>
                {
                    b.HasOne("ChatApp.Models.Group", "Group")
                        .WithMany("GroupInfos")
                        .HasForeignKey("GroupId")
                        .HasConstraintName("fk_group_infos_groups_group_id");

                    b.HasOne("ChatApp.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_group_infos_users_user_id");

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ChatApp.Models.UserActiveChat", b =>
                {
                    b.HasOne("ChatApp.Models.User", "User")
                        .WithMany("ActiveChats")
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_user_active_chats_users_user_id");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ChatApp.Models.Group", b =>
                {
                    b.Navigation("GroupInfos");
                });

            modelBuilder.Entity("ChatApp.Models.User", b =>
                {
                    b.Navigation("ActiveChats");
                });
#pragma warning restore 612, 618
        }
    }
}
