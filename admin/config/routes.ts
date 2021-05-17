﻿export default [
  {
    path: '/user',
    layout: false,
    routes: [
      {
        path: '/user',
        routes: [
          {
            name: 'login',
            path: '/user/login',
            component: './user/Login',
          },
        ],
      },
    ],
  },
  {
    path: '/welcome',
    name: 'welcome',
    icon: 'smile',
    component: './Welcome',
  },
  {
    path: '/admin',
    name: 'admin',
    icon: 'crown',
    component: './Admin',
    routes: [
      {
        path: '/admin/sub-page',
        name: 'sub-page',
        icon: 'smile',
        component: './Welcome',
      },
    ],
  },
  {
    name: 'users',
    icon: 'table',
    path: '/users',
    component: './user/list',
  },
  {
    name: 'userDetail',
    icon: 'table',
    path: '/users/detail/:id',
    hideInMenu: true,
    component: './user/detail',
  },
  {
    name: 'roles',
    icon: 'table',
    path: '/roles',
    component: './user/password',
  },
  {
    name: 'apps',
    icon: 'table',
    path: '/apps',
    component: './user/password',
  },
  {
    name: 'envs',
    icon: 'table',
    path: '/envs',
    component: './user/password',
  },
  {
    name: 'files',
    icon: 'table',
    path: '/files',
    component: './user/password',
  },
  {
    path: '/',
    redirect: '/welcome',
  },
  {
    name: 'password',
    icon: 'table',
    path: '/password',
    hideInMenu: true,
    component: './user/password',
  },
  {
    component: './404',
  },
];
