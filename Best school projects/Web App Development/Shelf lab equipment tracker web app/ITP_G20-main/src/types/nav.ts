import {Icons} from "@/components/nav/icons"

export interface NavItem {
    title: string
    href?: string
    parentLink?: string
    disabled?: boolean
    external?: boolean
    icon?: keyof typeof Icons
    label?: string
}

export interface NavItemWithChildren extends NavItem {
    items: NavItemWithChildren[]
}

export interface MainNavItem extends NavItem {
}

export interface SidebarNavItem extends NavItemWithChildren {
}