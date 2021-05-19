import Cookies from 'js-cookie';
import moment from 'moment';

/* eslint no-useless-escape:0 import/prefer-default-export:0 */
const reg = /(((^https?:(?:\/\/)?)(?:[-;:&=\+\$,\w]+@)?[A-Za-z0-9.-]+(?::\d+)?|(?:www.|[-;:&=\+\$,\w]+@)[A-Za-z0-9.-]+)((?:\/[\+~%\/.\w-_]*)?\??(?:[-\+=&;%@.\w_]*)#?(?:[\w]*))?)$/;

export const isUrl = (path: string): boolean => reg.test(path);

export const isAntDesignPro = (): boolean => {
  if (ANT_DESIGN_PRO_ONLY_DO_NOT_USE_IN_YOUR_PRODUCTION === 'site') {
    return true;
  }
  return window.location.hostname === 'preview.pro.ant.design';
};

// 给官方演示站点用，用于关闭真实开发环境不需要使用的特性
export const isAntDesignProOrDev = (): boolean => {
  const { NODE_ENV } = process.env;
  if (NODE_ENV === 'development') {
    return true;
  }
  return isAntDesignPro();
};

/**
 * 设置token
 * @param token  token
 * @param expires  过期时间
 */
export const setAccessToken = (token: string, expires: number) => {
  Cookies.set('assess_token', token, { expires: expires / (24 * 60 * 60) });
};

/**
 * 清除token
 */
export const clearAccessToken = () => {
  Cookies.set('assess_token', '', { expires: 0 });
};

/**
 * 获取token
 * @returns 获取token
 */
export const getAccessToken = () => Cookies.get('assess_token') || false;

/**
 * 获取baseUrl
 * @returns baseUrl
 */
export const getBaseUrl = () => "http://localhost:5000";

/**
 * 常用正则
 */
export const Regexs = {
  /**
   * 资源名称
   */
  resourceName: /^[0-9a-zA-Z_]{1,32}$/
}

/**
 * long转换为时间字符串
 * @param time 转换
 * @returns 时间字符串
 */
export const toTime = (time: number): string => {
  return moment(time * 1000).format('YYYY-MM-DD HH:mm')
}

