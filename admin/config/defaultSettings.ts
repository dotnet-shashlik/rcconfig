import { Settings as LayoutSettings } from '@ant-design/pro-layout';

const Settings: LayoutSettings & {
  pwa?: boolean;
  logo?: string;
} = {
  navTheme: 'light',
  // 拂晓蓝
  primaryColor: '#1890ff',
  layout: 'mix',
  contentWidth: 'Fluid',
  fixedHeader: false,
  fixSiderbar: true,
  colorWeak: false,
  title: 'Shashlik.RC',
  pwa: false,
  logo: 'https://img.youxi369.com/article/contents/2021/02/10/small_20210210112721737.jpeg',
  iconfontUrl: '',
};

export default Settings;
