"use client"

import {Menu} from "antd";
import React, {useState} from "react";
import {usePathname, useRouter} from "next/navigation";
import {ItemType} from "antd/es/menu/interface";
import {logout} from "@/app/lib/api";
import '@ant-design/v5-patch-for-react-19';

export const NavBar = ({items}: {items: ItemType[]}) => {

  const [current, setCurrent] = useState(usePathname())
  const router = useRouter()

  const clickMenu = (e: { key: string }) => {
    // Handle logout separately
    if (e.key === "/logout")
      return logout().then(() => {
        router.replace("/login")
        router.refresh()
      })
    setCurrent(e.key)
    router.push(e.key)
  }

  return (
    <Menu
      selectedKeys={[current]}
      mode="horizontal"
      onClick={clickMenu}
      items={items}
    />
  )
}

export default NavBar