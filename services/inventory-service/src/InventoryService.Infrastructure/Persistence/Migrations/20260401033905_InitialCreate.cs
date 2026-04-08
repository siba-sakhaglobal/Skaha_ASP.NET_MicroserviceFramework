using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "batches",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    batch_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    manufacture_date = table.Column<DateOnly>(type: "date", nullable: true),
                    expiry_date = table.Column<DateOnly>(type: "date", nullable: true),
                    shelf_life_days = table.Column<int>(type: "integer", nullable: true),
                    warehouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    rate = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_batches", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "gate_inwards",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    gate_inward_no = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    gate_inward_date = table.Column<DateOnly>(type: "date", nullable: false),
                    gate_inward_time = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: false),
                    site_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    reference_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    reference_doc_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reference_doc_no = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    vendor_customer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    vendor_customer_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    gstin = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    transporter_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    vehicle_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    number_of_pieces = table.Column<int>(type: "integer", nullable: true),
                    inward_value = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DriverName = table.Column<string>(type: "text", nullable: true),
                    DriverContact = table.Column<string>(type: "text", nullable: true),
                    LrNumber = table.Column<string>(type: "text", nullable: true),
                    LrDate = table.Column<DateOnly>(type: "date", nullable: true),
                    EwayBillNumber = table.Column<string>(type: "text", nullable: true),
                    Attachments = table.Column<string>(type: "text", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    modified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gate_inwards", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "inventory_settings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    allow_negative_stock = table.Column<bool>(type: "boolean", nullable: false),
                    gate_inward_mandatory = table.Column<bool>(type: "boolean", nullable: false),
                    qc_mandatory_on_grn = table.Column<bool>(type: "boolean", nullable: false),
                    default_valuation_method = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    auto_batch_generation = table.Column<bool>(type: "boolean", nullable: false),
                    batch_number_format = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    auto_serial_generation = table.Column<bool>(type: "boolean", nullable: false),
                    serial_number_format = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_inventory_settings", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "item_warehouse_mappings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    mapping_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    item_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    warehouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    warehouse_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    warehouse_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    warehouse_bin_id = table.Column<Guid>(type: "uuid", nullable: true),
                    bin_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    traceability_level = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    opening_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    opening_rate = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    opening_stock_date = table.Column<DateOnly>(type: "date", nullable: true),
                    min_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    reorder_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    max_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    modified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_item_warehouse_mappings", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "serial_numbers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    serial_no = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    batch_id = table.Column<Guid>(type: "uuid", nullable: true),
                    warehouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    bin_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    current_doc_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    current_doc_id = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_serial_numbers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "stock_adjustments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    adjustment_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    adjustment_date = table.Column<DateOnly>(type: "date", nullable: false),
                    adjustment_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    warehouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reason_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    modified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_adjustments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "stock_balances",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    warehouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    bin_id = table.Column<Guid>(type: "uuid", nullable: true),
                    batch_id = table.Column<Guid>(type: "uuid", nullable: true),
                    on_hand_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    reserved_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    average_cost = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    uom = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    modified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_balances", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "stock_ledger_entries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    warehouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    bin_id = table.Column<Guid>(type: "uuid", nullable: true),
                    batch_id = table.Column<Guid>(type: "uuid", nullable: true),
                    movement_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    document_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    document_id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    transaction_date = table.Column<DateOnly>(type: "date", nullable: false),
                    in_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    out_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    balance_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    rate = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    value = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    uom = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_ledger_entries", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "stock_transfers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    transfer_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    transfer_date = table.Column<DateOnly>(type: "date", nullable: false),
                    source_warehouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_warehouse_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    destination_warehouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    destination_warehouse_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    modified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_transfers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "gate_inward_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    gate_inward_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    item_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    uom = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    modified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_gate_inward_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_gate_inward_items_gate_inwards_gate_inward_id",
                        column: x => x.gate_inward_id,
                        principalTable: "gate_inwards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "goods_received_notes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    grn_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    grn_date = table.Column<DateOnly>(type: "date", nullable: false),
                    grn_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    site_id = table.Column<Guid>(type: "uuid", nullable: false),
                    site_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    warehouse_id = table.Column<Guid>(type: "uuid", nullable: true),
                    warehouse_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    vendor_id = table.Column<Guid>(type: "uuid", nullable: true),
                    vendor_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    customer_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    receipt_reason = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    internal_ref_no = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    approved_by = table.Column<Guid>(type: "uuid", nullable: true),
                    returnable_dc_id = table.Column<Guid>(type: "uuid", nullable: true),
                    supplier_challan_no = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    supplier_challan_date = table.Column<DateOnly>(type: "date", nullable: true),
                    gate_inward_id = table.Column<Guid>(type: "uuid", nullable: true),
                    exchange_rate = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    basic_value = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    tax_amount = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    additional_charges_total = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    total_grn_value = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    additional_charges_struct_id = table.Column<Guid>(type: "uuid", nullable: true),
                    customer_gstin = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    contact_person = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    contact_email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    contact_phone = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    created_by_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    attachments = table.Column<string>(type: "text", nullable: true),
                    transporter_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    vehicle_number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    driver_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    driver_contact = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    eway_bill_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    modified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_goods_received_notes", x => x.id);
                    table.ForeignKey(
                        name: "FK_goods_received_notes_gate_inwards_gate_inward_id",
                        column: x => x.gate_inward_id,
                        principalTable: "gate_inwards",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "stock_adjustment_lines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    stock_adjustment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    item_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    rate = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    uom = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    bin_id = table.Column<Guid>(type: "uuid", nullable: true),
                    batch_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    modified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_adjustment_lines", x => x.id);
                    table.ForeignKey(
                        name: "FK_stock_adjustment_lines_stock_adjustments_stock_adjustment_id",
                        column: x => x.stock_adjustment_id,
                        principalTable: "stock_adjustments",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "stock_transfer_lines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    stock_transfer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    item_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    requested_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    dispatched_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    received_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    uom = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    batch_id = table.Column<Guid>(type: "uuid", nullable: true),
                    source_bin_id = table.Column<Guid>(type: "uuid", nullable: true),
                    dest_bin_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    modified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_transfer_lines", x => x.id);
                    table.ForeignKey(
                        name: "FK_stock_transfer_lines_stock_transfers_stock_transfer_id",
                        column: x => x.stock_transfer_id,
                        principalTable: "stock_transfers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "grn_line_items",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    grn_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_id = table.Column<Guid>(type: "uuid", nullable: false),
                    item_code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    item_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    purchase_order_line_id = table.Column<Guid>(type: "uuid", nullable: true),
                    ordered_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    received_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    accepted_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    rejected_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    uom = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    unit_rate = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    basic_amount = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    tax_amount = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    total_amount = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    warehouse_id = table.Column<Guid>(type: "uuid", nullable: false),
                    bin_id = table.Column<Guid>(type: "uuid", nullable: true),
                    batch_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    serial_numbers = table.Column<string>(type: "text", nullable: true),
                    remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    sales_order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    qc_required = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    qc_status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    condition_on_return = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    tag_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    tax_rate = table.Column<decimal>(type: "numeric(6,2)", precision: 6, scale: 2, nullable: false),
                    additional_charges = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    dim_length = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    dim_width = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    dim_thickness = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    dim_pieces = table.Column<int>(type: "integer", nullable: true),
                    calculated_weight = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    modified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    modified_by = table.Column<Guid>(type: "uuid", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_grn_line_items", x => x.id);
                    table.ForeignKey(
                        name: "FK_grn_line_items_goods_received_notes_grn_id",
                        column: x => x.grn_id,
                        principalTable: "goods_received_notes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "idx_batch_tenant_number_item",
                table: "batches",
                columns: new[] { "tenant_id", "batch_number", "item_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_gate_inward_items_gate_inward_id",
                table: "gate_inward_items",
                column: "gate_inward_id");

            migrationBuilder.CreateIndex(
                name: "ix_goods_received_notes_customer_id",
                table: "goods_received_notes",
                column: "customer_id");

            migrationBuilder.CreateIndex(
                name: "ix_goods_received_notes_gate_inward_id",
                table: "goods_received_notes",
                column: "gate_inward_id");

            migrationBuilder.CreateIndex(
                name: "ix_goods_received_notes_grn_date",
                table: "goods_received_notes",
                column: "grn_date");

            migrationBuilder.CreateIndex(
                name: "ix_goods_received_notes_grn_number",
                table: "goods_received_notes",
                column: "grn_number");

            migrationBuilder.CreateIndex(
                name: "ix_goods_received_notes_tenant_site",
                table: "goods_received_notes",
                columns: new[] { "tenant_id", "site_id" });

            migrationBuilder.CreateIndex(
                name: "ix_goods_received_notes_tenant_type_status",
                table: "goods_received_notes",
                columns: new[] { "tenant_id", "grn_type", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_goods_received_notes_vendor_id",
                table: "goods_received_notes",
                column: "vendor_id");

            migrationBuilder.CreateIndex(
                name: "ix_grn_line_items_grn_id",
                table: "grn_line_items",
                column: "grn_id");

            migrationBuilder.CreateIndex(
                name: "ix_grn_line_items_item_id",
                table: "grn_line_items",
                column: "item_id");

            migrationBuilder.CreateIndex(
                name: "ix_grn_line_items_sales_order_id",
                table: "grn_line_items",
                column: "sales_order_id");

            migrationBuilder.CreateIndex(
                name: "ix_grn_line_items_warehouse_id",
                table: "grn_line_items",
                column: "warehouse_id");

            migrationBuilder.CreateIndex(
                name: "idx_settings_tenant",
                table: "inventory_settings",
                column: "tenant_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_iwm_tenant_item",
                table: "item_warehouse_mappings",
                columns: new[] { "tenant_id", "item_id" });

            migrationBuilder.CreateIndex(
                name: "idx_iwm_tenant_wh",
                table: "item_warehouse_mappings",
                columns: new[] { "tenant_id", "warehouse_id" });

            migrationBuilder.CreateIndex(
                name: "idx_iwm_unique",
                table: "item_warehouse_mappings",
                columns: new[] { "tenant_id", "item_id", "warehouse_id" },
                unique: true,
                filter: "is_deleted = false");

            migrationBuilder.CreateIndex(
                name: "idx_serial_tenant_no_item",
                table: "serial_numbers",
                columns: new[] { "tenant_id", "serial_no", "item_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_stock_adjustment_lines_stock_adjustment_id",
                table: "stock_adjustment_lines",
                column: "stock_adjustment_id");

            migrationBuilder.CreateIndex(
                name: "idx_sb_tenant_item_wh",
                table: "stock_balances",
                columns: new[] { "tenant_id", "item_id", "warehouse_id", "bin_id", "batch_id" });

            migrationBuilder.CreateIndex(
                name: "idx_sle_tenant_item_wh_date",
                table: "stock_ledger_entries",
                columns: new[] { "tenant_id", "item_id", "warehouse_id", "transaction_date" });

            migrationBuilder.CreateIndex(
                name: "IX_stock_transfer_lines_stock_transfer_id",
                table: "stock_transfer_lines",
                column: "stock_transfer_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "batches");

            migrationBuilder.DropTable(
                name: "gate_inward_items");

            migrationBuilder.DropTable(
                name: "grn_line_items");

            migrationBuilder.DropTable(
                name: "inventory_settings");

            migrationBuilder.DropTable(
                name: "item_warehouse_mappings");

            migrationBuilder.DropTable(
                name: "serial_numbers");

            migrationBuilder.DropTable(
                name: "stock_adjustment_lines");

            migrationBuilder.DropTable(
                name: "stock_balances");

            migrationBuilder.DropTable(
                name: "stock_ledger_entries");

            migrationBuilder.DropTable(
                name: "stock_transfer_lines");

            migrationBuilder.DropTable(
                name: "goods_received_notes");

            migrationBuilder.DropTable(
                name: "stock_adjustments");

            migrationBuilder.DropTable(
                name: "stock_transfers");

            migrationBuilder.DropTable(
                name: "gate_inwards");
        }
    }
}
