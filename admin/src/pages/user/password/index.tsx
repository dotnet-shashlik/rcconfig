import { PageContainer } from '@ant-design/pro-layout';
import { Form, Input, Button, message } from 'antd';
import { FormattedMessage, useIntl, useRequest } from 'umi';
import { changePassword } from '@/services/api/user';
import { clearAccessToken } from "@/utils/utils";

const layout = {
  labelCol: { span: 8 },
  wrapperCol: { span: 8 },
};
const tailLayout = {
  wrapperCol: { offset: 8, span: 16 },
};


export default () => {

  const changePasswordRequest = useRequest(changePassword, {
    manual: true, onSuccess: () => {
      message.success('success');
      clearAccessToken();
      window.location.href = '/user/login';
    }
  });

  const [form] = Form.useForm();

  return (
    <PageContainer>
      <Form
        form={form}
        {...layout}
        name="basic"
        initialValues={{ remember: true }}
        onFinish={changePasswordRequest.run}
      >
        <Form.Item
          label={useIntl().formatMessage({ id: 'app.settings.oldPassword' })}
          name="oldPassword"
          rules={[{ required: true }, { max: 32 }]}
        >
          <Input.Password />
        </Form.Item>

        <Form.Item
          label={useIntl().formatMessage({ id: 'app.settings.newPassword' })}
          name="newPassword"
          rules={[{ required: true }, { max: 32 }]}
        >
          <Input.Password />
        </Form.Item>
        <Form.Item
          label={useIntl().formatMessage({ id: 'app.settings.confirmPassword' })}
          name="confirmPassword"
          rules={[
            { required: true },
            { max: 32 },
            ({ getFieldValue }) => ({
              validator(_, value) {
                if (!value || getFieldValue('newPassword') === value) {
                  return Promise.resolve();
                }
                return Promise.reject(new Error('The two passwords that you entered do not match!'));
              },
            })
          ]}
        >
          <Input.Password />
        </Form.Item>

        <Form.Item {...tailLayout}>
          <Button type="primary" htmlType="submit">
            <FormattedMessage id="app.settings.submit" />
          </Button>
        </Form.Item>
      </Form>
    </PageContainer>);
};