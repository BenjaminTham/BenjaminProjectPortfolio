"use client"

import {Button, Form, message, Popconfirm, Select, Space, Table, Typography} from "antd";
import {useForm} from "antd/lib/form/Form";
import {deleteBiometrics, deleteUser, getUsers, updateUser} from "@/app/lib/api";
import {useEffect, useState} from "react";
import {GetUsers} from "@/app/lib/types";
import {ExclamationCircleFilled} from "@ant-design/icons";
import UserModal from "@/app/(authenticated)/users/UserModal";
import {BiometricsModal} from "@/app/(authenticated)/users/BiometricsModal";
const { Title } = Typography;


export default function Users() {

  const [form] = useForm()
  const [users, setUsers] = useState<GetUsers["data"]>([])
  const [displayMessage, contextHolder] = message.useMessage()
  const [loading, setLoading] = useState(false)
  const [selected, setSelected] = useState<GetUsers["data"][number] | null>(null)
  const [open, setOpen] = useState(false)
  const [openBiometrics, setOpenBiometrics] = useState(false)

  useEffect(() => {
    loadUsers()
  }, [])

  const clickSearch = () => {
    form.validateFields().then(values => loadUsers(values))
  }

  const clickReset = () => {
    form.resetFields()
    loadUsers()
  }

  const columns = [
    {title: "ID", dataIndex: "id"},
    {title: "Username", dataIndex: "username"},
    {title: "Email", dataIndex: "email"},
    {title: "Role", dataIndex: "role"},
    {title: "Status", dataIndex: "account_locked", render: (locked: boolean, _: GetUsers["data"][number]) => locked ? <Button onClick={() => clickUnlock(_.id)}>Unlock</Button>: "Active"},
    {title: "Location", dataIndex: "location"},
    {title: "Biometrics", dataIndex: "biometrics_enrolled", render: (enrolled: boolean, _: GetUsers["data"][number]) => !enrolled ?
        <Button onClick={() => clickEnroll(_)}>Enroll</Button>:
        <Popconfirm
          title="Remove Biometrics"
          description="Are you sure you want to remove biometrics for this user? Access to all facilities will be revoked."
          onConfirm={() => clickRemove(_.id)}
          icon={<ExclamationCircleFilled style={{color: "red"}}/>}
        >
          <Button danger>Remove</Button>
        </Popconfirm>
    },
    {title: "Alerts", dataIndex: "alert_subscribed", render: (subscribed: boolean) => subscribed ? "Subscribed": "Not subscribed"},
    {title: "Actions", dataIndex: "id", render: (id: number) =>
      <Space.Compact>
        <Button type="primary" onClick={() => clickEdit(id)}>Edit</Button>
        <Popconfirm
          title="Delete"
          description="Are you sure you want to delete this user?"
          onConfirm={() => clickDelete(id)}
          icon={<ExclamationCircleFilled style={{color: "red"}}/>}
        >
          <Button danger>Delete</Button>
        </Popconfirm>
      </Space.Compact>
    }
  ]

  const loadUsers = (values = {}) => {
    setLoading(true)
    getUsers(values).then(response => {
      setLoading(false)
      if (response.status === 200)
        return setUsers((response.body as GetUsers).data)
      displayMessage.error(response.body.message).then()
    })
  }

  const clickAdd = () => {
    setOpen(true)
  }

  const clickEdit = (userId: number) => {
    const user = users.find(user => user.id === userId)
    setSelected(user ?? null)
    setOpen(true)
  }

  const clickDelete = (userId: number) => {
    deleteUser(userId).then(response => {
      if (response.status === 200)
        return loadUsers()
      displayMessage.error(response.body.message).then()
    })
  }

  const clickUnlock = (userId: number) => {
    updateUser(userId, {unlock_account: true}).then(response => {
      if (response.status === 200) {
        displayMessage.success(response.body.message).then()
        return loadUsers()
      }
      displayMessage.error(response.body.message).then()
    })
  }

  const clickRemove = (userId: number) => {
    deleteBiometrics(userId).then(response => {
      if (response.status === 200) {
        displayMessage.success(response.body.message).then()
        return loadUsers()
      }
      displayMessage.error(response.body.message).then()
    })
  }

  const clickEnroll = (user: GetUsers["data"][number]) => {
    setSelected(user)
    setOpenBiometrics(true)
  }

  return (
    <div className="p-8 flex flex-col gap-4 max-w-5xl m-auto">
      {contextHolder}
      <Typography>
        <Title level={2}>Manage Users</Title>
      </Typography>
      <Form
        form={form}
        labelCol={{span: 8}}
        wrapperCol={{span: 16}}
        style={{maxWidth: 400}}
      >
        {/*<Form.Item*/}
        {/*  label="User ID"*/}
        {/*  name="id"*/}
        {/*>*/}
        {/*  <Input/>*/}
        {/*</Form.Item>*/}
        {/*<Form.Item*/}
        {/*  label="Username"*/}
        {/*  name="username"*/}
        {/*>*/}
        {/*  <Input/>*/}
        {/*</Form.Item>*/}
        {/*<Form.Item*/}
        {/*  label="Email"*/}
        {/*  name="email"*/}
        {/*>*/}
        {/*  <Input/>*/}
        {/*</Form.Item>*/}
        <Form.Item
          label="Role"
          name="role"
        >
          <Select>
            <Select.Option value="admin">Admin</Select.Option>
            <Select.Option value="security">Security</Select.Option>
            <Select.Option value="personnel">Personnel</Select.Option>
          </Select>
        </Form.Item>
        <Form.Item
          wrapperCol={{offset: 8, span: 16}}
        >
          <Space>
            <Button type="primary" onClick={clickSearch}>Search</Button>
            <Button onClick={clickReset}>Reset</Button>
          </Space>
        </Form.Item>
      </Form>
      <Button type="primary" className="self-end" onClick={clickAdd}>Add User</Button>
      <Table
        dataSource={users.map(user => ({...user, key: user.id}))}
        columns={columns}
        pagination={false}
        loading={loading}
      />
      <UserModal
        open={open}
        setOpen={setOpen}
        selected={selected}
        setSelected={setSelected}
        reloadUsers={loadUsers}
        displayMessage={displayMessage}
      />
      <BiometricsModal
        open={openBiometrics}
        setOpen={setOpenBiometrics}
        selected={selected}
        setSelected={setSelected}
        reloadUsers={loadUsers}
        displayMessage={displayMessage}
      />
    </div>
  );
}