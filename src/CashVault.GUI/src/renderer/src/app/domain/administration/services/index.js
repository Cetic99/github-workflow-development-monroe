/* eslint-disable prettier/prettier */
/* eslint-disable react/prop-types */

import { isEmpty } from 'lodash'

export const preparePermissionsData = (operatorPermissions, allPermissions) => {
  if (isEmpty(allPermissions)) {
    return []
  }

  const permissions = allPermissions.map((perm) => ({
    id: perm?.id,
    code: perm?.code,
    active: operatorPermissions?.find((p) => p?.code === perm?.code) || false
  }))

  const grouped = []

  for (let i = 0; i < permissions.length; i += 2) {
    grouped.push(permissions.slice(i, i + 2))
  }

  return grouped
}

export const isCardStepFailed = (step) => {
  return step?.initiated === true &&
    step?.processing === false &&
    step?.success === false;
}

export const formatDateString = (str) => {
  if (str == null || str == undefined || str == '')
    return "/"

  const date = new Date(str);

  const day = String(date.getDate()).padStart(2, '0');
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const year = date.getFullYear();

  return `${day}.${month}.${year}`;
}