using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SwanseaUniversityComputerScienceApp.Data.Migrations
{
    public partial class EditPostModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropColumn(
            //    name: "PostType",
            //    table: "Post");

            //migrationBuilder.AlterColumn<string>(
            //    name: "PostName",
            //    table: "Post",
            //    nullable: false,
            //    oldClrType: typeof(string),
            //    oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PostName",
                table: "Post",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<string>(
                name: "PostType",
                table: "Post",
                nullable: true);
        }
    }
}
