export type StatusColor = 'green' | 'blue' | 'yellow' | 'red' | 'gray';

export interface SelectOption {
  value: string;
  label: string;
}

export interface BreadcrumbItem {
  label: string;
  href?: string;
}

export interface SidebarItem {
  label: string;
  href: string;
  icon: string;
  children?: SidebarItem[];
  requiredModule?: string;
}

export interface TenantConfig {
  tenantId: string;
  tenantName: string;
  enabledModules: string[];
  theme: {
    primaryColor: string;
    logo: string;
    template: 'modern' | 'classic' | 'minimal';
  };
  features: Record<string, boolean>;
}

export interface UserInfo {
  userId: string;
  email: string;
  name: string;
  role: string;
  permissions: string[];
  assignedSites: string[];
}
