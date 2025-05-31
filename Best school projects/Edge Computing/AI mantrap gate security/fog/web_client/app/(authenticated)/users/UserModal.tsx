import {useForm} from "antd/lib/form/Form";
import {Checkbox, Form, Input, Modal, Select} from "antd";
import {AddUser, GetUsers} from "@/app/lib/types";
import {Dispatch, SetStateAction, useEffect} from "react";
import {addUser, updateUser} from "@/app/lib/api";
import {MessageInstance} from "antd/lib/message/interface";

export const UserModal = (
  {open, setOpen, selected, setSelected, reloadUsers, displayMessage}: {open: boolean, setOpen: Dispatch<SetStateAction<boolean>>, selected: GetUsers["data"][number] | null, setSelected: Dispatch<SetStateAction<GetUsers["data"][number] | null>>, reloadUsers: () => void, displayMessage: MessageInstance}
) => {

  const [form] = useForm()
  const addMode = selected === null

  const clickOk = () => {
    form.validateFields().then(values => {
      // Replace empty values with null
      values = Object.fromEntries(Object.entries(values).map(([key, value]) => [key, value !== "" ? value: null]));
      (addMode ? addUser(values): updateUser(selected.id, values)).then(response => {
        if (response.status === 200) {
          displayMessage.info(response.body.message).then()
          setOpen(false)
          setSelected(null)
          reloadUsers()
          form.resetFields()
          return
        }
        // Show overall error message
        if (response.body.message)
          displayMessage.error(response.body.message).then()
        // Update errors for specific fields
        const errors = (response.body as AddUser).errors
        if (errors) {
          // eslint-disable-next-line @typescript-eslint/ban-ts-comment
          // @ts-expect-error
          form.setFields(Object.entries(errors).map(([key, value]) => ({name: key, errors: [value]})));
        }
      })
    })
  }

  useEffect(() => {
    if (!addMode) {
      form.setFields([
        {name: "username", value: selected.username},
        {name: "email", value: selected.email},
        {name: "role", value: selected.role},
        {name: "alert_subscribed", value: selected.alert_subscribed},
        {name: "password", value: ""}
      ])
    }
  }, [open])



  return (
    <Modal
      title={addMode ? "Add User": "Edit User"}
      open={open}
      onOk={clickOk}
      onCancel={() => {
        setSelected(null)
        setOpen(false)
        form.resetFields()
      }}
      okText={addMode ? "Add": "Update"}
    >
      <Form
        form={form}
        labelCol={{span: 8}}
        wrapperCol={{span: 16}}
      >
        <Form.Item
          label="Username"
          name="username"
        >
          <Input/>
        </Form.Item>
        <Form.Item
          label="Email"
          name="email"
        >
          <Input/>
        </Form.Item>
        <Form.Item
          label="Password"
          name="password"
        >
          <Input.Password/>
        </Form.Item>
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
          label="Subscribe to Alerts"
          name="alert_subscribed"
          valuePropName="checked"
        >
          <Checkbox/>
        </Form.Item>
      </Form>
    </Modal>
  )

}

export default UserModal