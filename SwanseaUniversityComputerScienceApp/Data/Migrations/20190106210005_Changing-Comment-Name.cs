using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace SwanseaUniversityComputerScienceApp.Data.Migrations
{
    public partial class ChangingCommentName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CommentInformation",
                table: "Comment",
                newName: "CommentContent");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CommentContent",
                table: "Comment",
                newName: "CommentInformation");
        }
    }
}
