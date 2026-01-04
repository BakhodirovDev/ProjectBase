using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FirstMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "enum_status",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    short_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    full_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_enum_status", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "enum_auth_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    short_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    full_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_enum_auth_type", x => x.id);
                    table.ForeignKey(
                        name: "fk_enum_auth_type_enum_status_status_id",
                        column: x => x.status_id,
                        principalTable: "enum_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "enum_document_type",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    short_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    full_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_enum_document_type", x => x.id);
                    table.ForeignKey(
                        name: "fk_enum_document_type_enum_status_status_id",
                        column: x => x.status_id,
                        principalTable: "enum_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "enum_gender",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    short_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    full_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_enum_gender", x => x.id);
                    table.ForeignKey(
                        name: "fk_enum_gender_enum_status_status_id",
                        column: x => x.status_id,
                        principalTable: "enum_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "info_country",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    short_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    full_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_info_country", x => x.id);
                    table.ForeignKey(
                        name: "fk_info_country_enum_status_status_id",
                        column: x => x.status_id,
                        principalTable: "enum_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "info_nationality",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    short_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    full_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_info_nationality", x => x.id);
                    table.ForeignKey(
                        name: "fk_info_nationality_enum_status_status_id",
                        column: x => x.status_id,
                        principalTable: "enum_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "permission",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    resource = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    action = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_permission", x => x.id);
                    table.ForeignKey(
                        name: "fk_permission_enum_status_status_id",
                        column: x => x.status_id,
                        principalTable: "enum_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "role",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    is_default = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_system_role = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_system_admin = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role", x => x.id);
                    table.ForeignKey(
                        name: "fk_role_enum_status_status_id",
                        column: x => x.status_id,
                        principalTable: "enum_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "info_region",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    soato = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    roaming_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    short_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    full_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    country_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_info_region", x => x.id);
                    table.ForeignKey(
                        name: "fk_info_region_enum_status_status_id",
                        column: x => x.status_id,
                        principalTable: "enum_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_info_region_info_country_country_id",
                        column: x => x.country_id,
                        principalTable: "info_country",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "role_permission",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    permission_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_permission", x => x.id);
                    table.ForeignKey(
                        name: "fk_role_permission_enum_status_status_id",
                        column: x => x.status_id,
                        principalTable: "enum_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_role_permission_permission_permission_id",
                        column: x => x.permission_id,
                        principalTable: "permission",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_role_permission_role_role_id",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "info_district",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    soato = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    roaming_code = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    short_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    full_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    is_center = table.Column<bool>(type: "boolean", nullable: false),
                    region_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_info_district", x => x.id);
                    table.ForeignKey(
                        name: "fk_info_district_enum_status_status_id",
                        column: x => x.status_id,
                        principalTable: "enum_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_info_district_info_region_region_id",
                        column: x => x.region_id,
                        principalTable: "info_region",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "info_mfy",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    short_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    full_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    region_id = table.Column<int>(type: "integer", nullable: false),
                    district_id = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_info_mfy", x => x.id);
                    table.ForeignKey(
                        name: "fk_info_mfy_enum_status_status_id",
                        column: x => x.status_id,
                        principalTable: "enum_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_info_mfy_info_district_district_id",
                        column: x => x.district_id,
                        principalTable: "info_district",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_info_mfy_info_region_region_id",
                        column: x => x.region_id,
                        principalTable: "info_region",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "person",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    middle_name = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    last_name = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    short_name = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    full_name = table.Column<string>(type: "character varying(75)", maxLength: 75, nullable: false),
                    gender_id = table.Column<int>(type: "integer", nullable: false),
                    photo_id = table.Column<Guid>(type: "uuid", nullable: true),
                    nationality_id = table.Column<int>(type: "integer", nullable: false),
                    country_id = table.Column<int>(type: "integer", nullable: false),
                    region_id = table.Column<int>(type: "integer", nullable: false),
                    district_id = table.Column<int>(type: "integer", nullable: false),
                    mfy_id = table.Column<int>(type: "integer", nullable: true),
                    address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_person", x => x.id);
                    table.ForeignKey(
                        name: "fk_person_enum_gender_gender_id",
                        column: x => x.gender_id,
                        principalTable: "enum_gender",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_person_enum_status_status_id",
                        column: x => x.status_id,
                        principalTable: "enum_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_person_info_country_country_id",
                        column: x => x.country_id,
                        principalTable: "info_country",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_person_info_district_district_id",
                        column: x => x.district_id,
                        principalTable: "info_district",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_person_info_mfy_mfy_id",
                        column: x => x.mfy_id,
                        principalTable: "info_mfy",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_person_info_nationality_nationality_id",
                        column: x => x.nationality_id,
                        principalTable: "info_nationality",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_person_info_region_region_id",
                        column: x => x.region_id,
                        principalTable: "info_region",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "person_phone_numbers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    phone_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_person_phone_numbers", x => x.id);
                    table.ForeignKey(
                        name: "fk_person_phone_numbers_enum_status_status_id",
                        column: x => x.status_id,
                        principalTable: "enum_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_person_phone_numbers_person_person_id",
                        column: x => x.person_id,
                        principalTable: "person",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    is_email_confirmed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    password_hash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_enum_status_status_id",
                        column: x => x.status_id,
                        principalTable: "enum_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_person_person_id",
                        column: x => x.person_id,
                        principalTable: "person",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "token",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    access_token_id = table.Column<Guid>(type: "uuid", nullable: false),
                    access_token_expire_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    refresh_token_id = table.Column<Guid>(type: "uuid", nullable: false),
                    refresh_token_expire_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_token", x => x.id);
                    table.ForeignKey(
                        name: "fk_token_enum_status_status_id",
                        column: x => x.status_id,
                        principalTable: "enum_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_token_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "user_role",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_role", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_role_enum_status_status_id",
                        column: x => x.status_id,
                        principalTable: "enum_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_user_role_role_role_id",
                        column: x => x.role_id,
                        principalTable: "role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_role_user_user_id",
                        column: x => x.user_id,
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "device_info",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    ip_address = table.Column<string>(type: "character varying(45)", maxLength: 45, nullable: false),
                    device_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    device_model = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    os_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    os_version = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    browser_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    browser_version = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    user_agent = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    is_bot = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_mobile = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_tablet = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_desktop = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    country_code = table.Column<string>(type: "character(2)", fixedLength: true, maxLength: 2, nullable: true),
                    last_activity_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    login_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    is_trusted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    device_nickname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    token_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    status_id = table.Column<int>(type: "integer", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_device_info", x => x.id);
                    table.ForeignKey(
                        name: "fk_device_info_enum_status_status_id",
                        column: x => x.status_id,
                        principalTable: "enum_status",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_device_info_token_id",
                        column: x => x.id,
                        principalTable: "token",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_device_info_status_id",
                table: "device_info",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "ix_device_infos_browser",
                table: "device_info",
                columns: new[] { "browser_name", "browser_version" });

            migrationBuilder.CreateIndex(
                name: "ix_device_infos_country_code",
                table: "device_info",
                column: "country_code",
                filter: "\"country_code\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_device_infos_device_flags",
                table: "device_info",
                columns: new[] { "is_mobile", "is_tablet", "is_desktop" });

            migrationBuilder.CreateIndex(
                name: "ix_device_infos_device_type_os_name",
                table: "device_info",
                columns: new[] { "device_type", "os_name" });

            migrationBuilder.CreateIndex(
                name: "ix_device_infos_ip_address",
                table: "device_info",
                column: "ip_address");

            migrationBuilder.CreateIndex(
                name: "ix_device_infos_ip_address_last_activity_at",
                table: "device_info",
                columns: new[] { "ip_address", "last_activity_at" });

            migrationBuilder.CreateIndex(
                name: "ix_device_infos_is_bot",
                table: "device_info",
                column: "is_bot",
                filter: "\"is_bot\" = true");

            migrationBuilder.CreateIndex(
                name: "ix_device_infos_is_trusted",
                table: "device_info",
                column: "is_trusted");

            migrationBuilder.CreateIndex(
                name: "ix_device_infos_last_activity_at",
                table: "device_info",
                column: "last_activity_at",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "ix_device_infos_token_id",
                table: "device_info",
                column: "token_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_enum_auth_type_code",
                table: "enum_auth_type",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_enum_auth_type_status_id",
                table: "enum_auth_type",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "ix_enum_document_type_code",
                table: "enum_document_type",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_enum_document_type_status_id",
                table: "enum_document_type",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "ix_enum_gender_code",
                table: "enum_gender",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_enum_gender_status_id",
                table: "enum_gender",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "ix_info_countries_code",
                table: "info_country",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_info_countries_short_name",
                table: "info_country",
                column: "short_name");

            migrationBuilder.CreateIndex(
                name: "ix_info_country_status_id",
                table: "info_country",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "ix_info_district_status_id",
                table: "info_district",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "ix_info_districts_code",
                table: "info_district",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_info_districts_region_id",
                table: "info_district",
                column: "region_id");

            migrationBuilder.CreateIndex(
                name: "ix_info_districts_short_name",
                table: "info_district",
                column: "short_name");

            migrationBuilder.CreateIndex(
                name: "ix_info_districts_soato_roaming_code",
                table: "info_district",
                columns: new[] { "soato", "roaming_code" });

            migrationBuilder.CreateIndex(
                name: "ix_info_mfy_status_id",
                table: "info_mfy",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "ix_info_mfys_code",
                table: "info_mfy",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_info_mfys_district_id",
                table: "info_mfy",
                column: "district_id");

            migrationBuilder.CreateIndex(
                name: "ix_info_mfys_region_id",
                table: "info_mfy",
                column: "region_id");

            migrationBuilder.CreateIndex(
                name: "ix_info_mfys_region_id_district_id",
                table: "info_mfy",
                columns: new[] { "region_id", "district_id" });

            migrationBuilder.CreateIndex(
                name: "ix_info_mfys_short_name",
                table: "info_mfy",
                column: "short_name");

            migrationBuilder.CreateIndex(
                name: "ix_info_nationalities_code",
                table: "info_nationality",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_info_nationalities_short_name",
                table: "info_nationality",
                column: "short_name");

            migrationBuilder.CreateIndex(
                name: "ix_info_nationality_status_id",
                table: "info_nationality",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "ix_info_region_status_id",
                table: "info_region",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "ix_info_regions_code",
                table: "info_region",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_info_regions_country_id",
                table: "info_region",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "ix_info_regions_short_name",
                table: "info_region",
                column: "short_name");

            migrationBuilder.CreateIndex(
                name: "ix_permission_status_id",
                table: "permission",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "ix_permissions_name",
                table: "permission",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_permissions_resource",
                table: "permission",
                column: "resource");

            migrationBuilder.CreateIndex(
                name: "ix_permissions_resource_action",
                table: "permission",
                columns: new[] { "resource", "action" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_person_mfy_id",
                table: "person",
                column: "mfy_id");

            migrationBuilder.CreateIndex(
                name: "ix_person_nationality_id",
                table: "person",
                column: "nationality_id");

            migrationBuilder.CreateIndex(
                name: "ix_person_status_id",
                table: "person",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "ix_persons_country_id",
                table: "person",
                column: "country_id");

            migrationBuilder.CreateIndex(
                name: "ix_persons_district_id",
                table: "person",
                column: "district_id");

            migrationBuilder.CreateIndex(
                name: "ix_persons_first_name_last_name",
                table: "person",
                columns: new[] { "first_name", "last_name" });

            migrationBuilder.CreateIndex(
                name: "ix_persons_full_name",
                table: "person",
                column: "full_name");

            migrationBuilder.CreateIndex(
                name: "ix_persons_gender_id",
                table: "person",
                column: "gender_id");

            migrationBuilder.CreateIndex(
                name: "ix_persons_region_id",
                table: "person",
                column: "region_id");

            migrationBuilder.CreateIndex(
                name: "ix_person_phone_numbers_person_id",
                table: "person_phone_numbers",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "ix_person_phone_numbers_person_id_phone_number",
                table: "person_phone_numbers",
                columns: new[] { "person_id", "phone_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_person_phone_numbers_phone_number",
                table: "person_phone_numbers",
                column: "phone_number");

            migrationBuilder.CreateIndex(
                name: "ix_person_phone_numbers_status_id",
                table: "person_phone_numbers",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "ix_role_status_id",
                table: "role",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "ix_roles_is_system_admin",
                table: "role",
                column: "is_system_admin");

            migrationBuilder.CreateIndex(
                name: "ix_roles_is_system_role",
                table: "role",
                column: "is_system_role");

            migrationBuilder.CreateIndex(
                name: "ix_roles_name",
                table: "role",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_role_permission_status_id",
                table: "role_permission",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "ix_role_permissions_permission_id",
                table: "role_permission",
                column: "permission_id");

            migrationBuilder.CreateIndex(
                name: "ix_role_permissions_role_id",
                table: "role_permission",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_role_permissions_role_id_permission_id",
                table: "role_permission",
                columns: new[] { "role_id", "permission_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_token_status_id",
                table: "token",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "ix_tokens_access_token_id",
                table: "token",
                column: "access_token_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tokens_refresh_token_id",
                table: "token",
                column: "refresh_token_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tokens_user_id",
                table: "token",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_status_id",
                table: "user",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_email",
                table: "user",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "ix_users_person_id",
                table: "user",
                column: "person_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_user_name",
                table: "user",
                column: "user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_role_status_id",
                table: "user_role",
                column: "status_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_role_id",
                table: "user_role",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_user_id",
                table: "user_role",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_user_id_role_id",
                table: "user_role",
                columns: new[] { "user_id", "role_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "device_info");

            migrationBuilder.DropTable(
                name: "enum_auth_type");

            migrationBuilder.DropTable(
                name: "enum_document_type");

            migrationBuilder.DropTable(
                name: "person_phone_numbers");

            migrationBuilder.DropTable(
                name: "role_permission");

            migrationBuilder.DropTable(
                name: "user_role");

            migrationBuilder.DropTable(
                name: "token");

            migrationBuilder.DropTable(
                name: "permission");

            migrationBuilder.DropTable(
                name: "role");

            migrationBuilder.DropTable(
                name: "user");

            migrationBuilder.DropTable(
                name: "person");

            migrationBuilder.DropTable(
                name: "enum_gender");

            migrationBuilder.DropTable(
                name: "info_mfy");

            migrationBuilder.DropTable(
                name: "info_nationality");

            migrationBuilder.DropTable(
                name: "info_district");

            migrationBuilder.DropTable(
                name: "info_region");

            migrationBuilder.DropTable(
                name: "info_country");

            migrationBuilder.DropTable(
                name: "enum_status");
        }
    }
}
