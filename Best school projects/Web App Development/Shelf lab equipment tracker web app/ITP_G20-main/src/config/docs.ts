import {MainNavItem, SidebarNavItem} from "@/types/nav"

interface DocsConfig {
    mainNav: MainNavItem[]
    sidebarNav: SidebarNavItem[]
}

export const docsConfig: DocsConfig = {
    mainNav: [
        {
            title: "Rack Management",
            href: "/rack",
        },
        {
            title: "Tray Management",
            href: "/tray/manage",
            parentLink: "tray",
        },
        {
            title: "Config",
            href: "/config/profile",
            parentLink: "config",
        },
    ],
    // This is where you define additional links in the sidebar
    sidebarNav: []
}

export const configItems = [
    {
        title: "Profile",
        href: "/config/profile",
    },
    {
        title: "Reader",
        href: "/config/reader",
    },
    {
        title: "Host IP",
        href: "/config/host-ip",
    },
    {
        title: "Power Level",
        href: "/config/power-level",
    },
    {
        title: "Appearance",
        href: "/config/appearance",
    }
]

export const trayItems = [
    {
        title: "Manage Trays",
        href: "/tray/manage",
    },
    {
        title: "Read Trays",
        href: "/tray/read",
    },
    {
        title: "Create Trays",
        href: "/tray/create",
    },
]