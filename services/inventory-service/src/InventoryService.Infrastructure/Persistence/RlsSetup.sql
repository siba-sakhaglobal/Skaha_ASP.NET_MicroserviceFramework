-- ============================================================
-- PostgreSQL Row-Level Security (RLS) for Shared Database
-- ============================================================
-- This SQL is applied to the SHARED inventory_db only.
-- Dedicated tenant databases do NOT need RLS (entire DB = one tenant).
--
-- Defense-in-depth: EF Core global query filters are the first layer.
-- PostgreSQL RLS is the second layer — prevents data leaks even if
-- application code bypasses EF Core filters (raw SQL, bugs, etc.).
--
-- Architecture doc Section 7: "Row-level security (shared DB)"
-- ============================================================

-- 1. Create a session variable for the current tenant.
--    The application sets this via: SET app.current_tenant_id = '<uuid>';
--    This is done by the DbContext connection interceptor.

-- 2. Enable RLS on all inventory tables

ALTER TABLE item_warehouse_mappings ENABLE ROW LEVEL SECURITY;
ALTER TABLE gate_inwards ENABLE ROW LEVEL SECURITY;
ALTER TABLE gate_inward_items ENABLE ROW LEVEL SECURITY;
ALTER TABLE goods_received_notes ENABLE ROW LEVEL SECURITY;
ALTER TABLE grn_line_items ENABLE ROW LEVEL SECURITY;
ALTER TABLE stock_balances ENABLE ROW LEVEL SECURITY;
ALTER TABLE stock_ledger_entries ENABLE ROW LEVEL SECURITY;
ALTER TABLE stock_transfers ENABLE ROW LEVEL SECURITY;
ALTER TABLE stock_transfer_lines ENABLE ROW LEVEL SECURITY;
ALTER TABLE stock_adjustments ENABLE ROW LEVEL SECURITY;
ALTER TABLE stock_adjustment_lines ENABLE ROW LEVEL SECURITY;
ALTER TABLE batches ENABLE ROW LEVEL SECURITY;
ALTER TABLE serial_numbers ENABLE ROW LEVEL SECURITY;
ALTER TABLE inventory_settings ENABLE ROW LEVEL SECURITY;

-- 3. Create RLS policies: tenant can only see their own rows

CREATE POLICY tenant_isolation_item_warehouse_mappings ON item_warehouse_mappings
    USING (tenant_id = current_setting('app.current_tenant_id')::uuid);

CREATE POLICY tenant_isolation_gate_inwards ON gate_inwards
    USING (tenant_id = current_setting('app.current_tenant_id')::uuid);

CREATE POLICY tenant_isolation_gate_inward_items ON gate_inward_items
    USING (tenant_id = current_setting('app.current_tenant_id')::uuid);

CREATE POLICY tenant_isolation_goods_received_notes ON goods_received_notes
    USING (tenant_id = current_setting('app.current_tenant_id')::uuid);

CREATE POLICY tenant_isolation_grn_line_items ON grn_line_items
    USING (tenant_id = current_setting('app.current_tenant_id')::uuid);

CREATE POLICY tenant_isolation_stock_balances ON stock_balances
    USING (tenant_id = current_setting('app.current_tenant_id')::uuid);

CREATE POLICY tenant_isolation_stock_ledger_entries ON stock_ledger_entries
    USING (tenant_id = current_setting('app.current_tenant_id')::uuid);

CREATE POLICY tenant_isolation_stock_transfers ON stock_transfers
    USING (tenant_id = current_setting('app.current_tenant_id')::uuid);

CREATE POLICY tenant_isolation_stock_transfer_lines ON stock_transfer_lines
    USING (tenant_id = current_setting('app.current_tenant_id')::uuid);

CREATE POLICY tenant_isolation_stock_adjustments ON stock_adjustments
    USING (tenant_id = current_setting('app.current_tenant_id')::uuid);

CREATE POLICY tenant_isolation_stock_adjustment_lines ON stock_adjustment_lines
    USING (tenant_id = current_setting('app.current_tenant_id')::uuid);

CREATE POLICY tenant_isolation_batches ON batches
    USING (tenant_id = current_setting('app.current_tenant_id')::uuid);

CREATE POLICY tenant_isolation_serial_numbers ON serial_numbers
    USING (tenant_id = current_setting('app.current_tenant_id')::uuid);

CREATE POLICY tenant_isolation_inventory_settings ON inventory_settings
    USING (tenant_id = current_setting('app.current_tenant_id')::uuid);

-- 4. Force RLS even for table owners (prevents bypass by superuser connections)
ALTER TABLE item_warehouse_mappings FORCE ROW LEVEL SECURITY;
ALTER TABLE gate_inwards FORCE ROW LEVEL SECURITY;
ALTER TABLE gate_inward_items FORCE ROW LEVEL SECURITY;
ALTER TABLE goods_received_notes FORCE ROW LEVEL SECURITY;
ALTER TABLE grn_line_items FORCE ROW LEVEL SECURITY;
ALTER TABLE stock_balances FORCE ROW LEVEL SECURITY;
ALTER TABLE stock_ledger_entries FORCE ROW LEVEL SECURITY;
ALTER TABLE stock_transfers FORCE ROW LEVEL SECURITY;
ALTER TABLE stock_transfer_lines FORCE ROW LEVEL SECURITY;
ALTER TABLE stock_adjustments FORCE ROW LEVEL SECURITY;
ALTER TABLE stock_adjustment_lines FORCE ROW LEVEL SECURITY;
ALTER TABLE batches FORCE ROW LEVEL SECURITY;
ALTER TABLE serial_numbers FORCE ROW LEVEL SECURITY;
ALTER TABLE inventory_settings FORCE ROW LEVEL SECURITY;

-- 5. IMPORTANT: The application DB user (erp_app) must NOT be a superuser.
--    Create a non-superuser role for the application:
--
--    CREATE ROLE erp_app WITH LOGIN PASSWORD '...' NOSUPERUSER;
--    GRANT ALL ON ALL TABLES IN SCHEMA public TO erp_app;
--    GRANT ALL ON ALL SEQUENCES IN SCHEMA public TO erp_app;
--
--    The migration user (erp_admin) CAN be superuser for schema changes,
--    but the application connection string must use erp_app.

-- 6. Application sets tenant context on each connection:
--    SET app.current_tenant_id = 'aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee';
--    This is done via an EF Core connection interceptor (see TenantConnectionInterceptor.cs).
