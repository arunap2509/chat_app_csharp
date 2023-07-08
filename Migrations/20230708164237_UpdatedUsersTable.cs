using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ChatApp.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedUsersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "friends");

            migrationBuilder.DropTable(
                name: "group_infos");

            migrationBuilder.DropTable(
                name: "user_active_chats");

            migrationBuilder.AddColumn<List<string>>(
                name: "friends_ids",
                table: "users",
                type: "text[]",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "phone_number",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "description",
                table: "groups",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "group_member_infos",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    group_id = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<string>(type: "text", nullable: true),
                    is_admin = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_group_member_infos", x => x.id);
                    table.ForeignKey(
                        name: "fk_group_member_infos_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_group_member_infos_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "threads",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: true),
                    channel_id = table.Column<string>(type: "text", nullable: true),
                    type = table.Column<short>(type: "smallint", nullable: false),
                    state = table.Column<short>(type: "smallint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_threads", x => x.id);
                    table.ForeignKey(
                        name: "fk_threads_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_group_member_infos_group_id",
                table: "group_member_infos",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "ix_group_member_infos_user_id",
                table: "group_member_infos",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_threads_user_id",
                table: "threads",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "group_member_infos");

            migrationBuilder.DropTable(
                name: "threads");

            migrationBuilder.DropColumn(
                name: "friends_ids",
                table: "users");

            migrationBuilder.DropColumn(
                name: "phone_number",
                table: "users");

            migrationBuilder.DropColumn(
                name: "description",
                table: "groups");

            migrationBuilder.CreateTable(
                name: "friends",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<string>(type: "text", nullable: true),
                    friends_ids = table.Column<List<string>>(type: "jsonb", nullable: true),
                    userd = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_friends", x => x.id);
                    table.ForeignKey(
                        name: "fk_friends_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "group_infos",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    group_id = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<string>(type: "text", nullable: true),
                    is_admin = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_group_infos", x => x.id);
                    table.ForeignKey(
                        name: "fk_group_infos_groups_group_id",
                        column: x => x.group_id,
                        principalTable: "groups",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_group_infos_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "user_active_chats",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: true),
                    channel_id = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_active_chats", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_active_chats_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "ix_friends_user_id",
                table: "friends",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_group_infos_group_id",
                table: "group_infos",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "ix_group_infos_user_id",
                table: "group_infos",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_active_chats_user_id",
                table: "user_active_chats",
                column: "user_id");
        }
    }
}
