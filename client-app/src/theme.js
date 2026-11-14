/** Shared Ant Design theme + brand tokens for NIT Company POS. */
export const brand = {
  name: "NIT Company",
  tagline: "Point of Sale",
  primary: "#0F766E",
  primaryHover: "#0D9488",
  ink: "#0F172A",
  muted: "#64748B",
  surface: "#F1F5F9",
  panel: "#FFFFFF",
};

export const appTheme = {
  token: {
    colorPrimary: brand.primary,
    colorInfo: brand.primary,
    colorSuccess: "#059669",
    colorWarning: "#D97706",
    colorError: "#DC2626",
    colorTextBase: brand.ink,
    colorBgLayout: brand.surface,
    borderRadius: 10,
    fontFamily:
      '"Plus Jakarta Sans", ui-sans-serif, system-ui, -apple-system, Segoe UI, sans-serif',
    controlHeight: 40,
  },
  components: {
    Layout: {
      headerBg: "transparent",
      bodyBg: brand.surface,
      siderBg: "#0B3D3A",
      triggerBg: "#0A332F",
    },
    Menu: {
      darkItemBg: "#0B3D3A",
      darkSubMenuItemBg: "#08302D",
      darkItemSelectedBg: brand.primary,
      darkItemHoverBg: "rgba(255,255,255,0.08)",
    },
    Card: {
      borderRadiusLG: 14,
    },
    Table: {
      headerBg: "#E2E8F0",
    },
    Button: {
      primaryShadow: "none",
    },
  },
};
