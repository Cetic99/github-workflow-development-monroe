export const Screen = {
  Idle: 'Idle'
}

const ScreenCollection = {
  [Screen.Idle]: () => import('@screens/parcel-locker/idle')
}

export default ScreenCollection
