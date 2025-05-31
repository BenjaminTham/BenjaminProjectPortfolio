import {getData} from "@/app/lib/api";
import NavBar from "@/app/(authenticated)/NavBar";
import {Button} from "antd";
import '@ant-design/v5-patch-for-react-19';
import React from "react";
import "./dashboard.css"

const Layout = async ({children}: { children: React.ReactNode }) => {

  const pages = [
    {label: "Dashboard", key: "/", roles: ["admin", "security"]},
    {label: "Users", key: "/users", roles: ["admin"]},
    {label: "Mantraps", key: "/mantraps", roles: ["admin", "security"]},
    {label: "Logs", key: "/logs", roles: ["admin", "security"]},
    {label: (<Button type="primary">Logout</Button>), key: "/logout", roles: ["admin", "security"], style: {marginLeft: "auto"}}
  ]
  const role = (await getData("user")).role

  return (
    <div>
      <NavBar items={pages.filter(page => page.roles.includes(role))} />
      {children}
    </div>
  )
}

export default Layout;