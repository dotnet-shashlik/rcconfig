export default [
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
    name: 'apps',
    icon: 'table',
    path: '/apps',
    component: './app',
  },
  {
    name: 'envs',
    icon: 'table',
    path: '/envs',
    component: './env',
  },
  {
    name: 'files',
    icon: 'table',
    path: '/files',
    component: './file',
  },
  {
    name: 'files-detail',
    icon: 'table',
    path: '/files/detail/:app/:env',
    hideInMenu: true,
    component: './file/detail',
  },
  {
    name: 'logs',
    icon: 'table',
    path: '/logs',
    component: './log',
  },
  {
    name: 'logs-detail',
    icon: 'table',
    path: '/logs/detail/:app/:env/:logId',
    hideInMenu: true,
    component: './log/detail',
  },
  {
    name: 'logs-byfileid',
    icon: 'table',
    path: '/logsbyfileid/:app/:env/:fileId',
    hideInMenu: true,
    component: './log/byFileId',
  },
  {
    name: 'users',
    icon: 'table',
    path: '/users',
    component: './user/list',
    access: 'admin'
  },
  {
    name: 'userDetail',
    icon: 'table',
    path: '/users/detail/:id',
    hideInMenu: true,
    component: './user/detail',
    access: 'admin'
  },
  {
    name: 'roles',
    icon: 'table',
    path: '/roles',
    component: './role',
    access: 'admin'
  },
  {
    name: 'resources',
    icon: 'table',
    path: '/resources',
    component: './resource',
    access: 'admin'
  },
  {
    name: 'secrets',
    icon: 'table',
    path: '/secrets',
    hideInMenu: true,
    component: './secret',
  },
  {
    path: '/',
    redirect: '/apps',
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
