"use client"

import {useForm} from "antd/lib/form/Form";
import {Button, Form, Input, message} from "antd";
import Image from 'next/image';
import Logo from "../../public/logo.png"
import PlatformName from "../../public/platform_name.png"
import {login} from "@/app/lib/api";
import {useRouter} from "next/navigation";
import '@ant-design/v5-patch-for-react-19';
import {useState} from "react";

export default function Login() {

  const [form] = useForm()
  const [displayMessage, contextHolder] = message.useMessage()
  const router = useRouter()
  const [loginDisabled, setLoginDisabled] = useState(false)

  const clickLogin = async () => {
    setLoginDisabled(true)
    form.validateFields().then(values => {
      login(values).then(response => {
        if (response.status === 200)
          return router.replace("/")
        setLoginDisabled(false)
        const body = response.body
        displayMessage.error(typeof(body) === "string" ? body: body.message)
      })
    }).catch(() => {})
  }

  return (
    <div className="flex min-h-screen flex-col items-center justify-center gap-8">
      {contextHolder}
      <div className="flex items-center justify-center gap-2">
        <Image src={Logo} alt="" className="h-24 w-auto border-r-2 border-black pr-2"/>
        <Image src={PlatformName} alt="GateGuard Security" className="h-24 w-auto"/>
      </div>
      <Form
        form={form}
        labelCol={{span: 8}}
        wrapperCol={{span: 16}}
        style={{minWidth: 400}}
      >
        <Form.Item
          label="Username"
          name="username"
          rules={[
            {required: true, message: "Username is required"}
          ]}
        >
          <Input/>
        </Form.Item>
        <Form.Item
          label="Password"
          name="password"
          rules={[
            {required: true, message: "Password is required"}
          ]}
        >
          <Input.Password/>
        </Form.Item>
        <Form.Item label={null}>
          <Button type="primary" onClick={clickLogin} disabled={loginDisabled}>Login</Button>
        </Form.Item>
      </Form>

    </div>
  )
}