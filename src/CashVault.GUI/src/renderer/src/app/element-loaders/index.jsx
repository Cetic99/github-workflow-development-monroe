/* eslint-disable prettier/prettier */
import { lazy, Suspense } from 'react'
import ComponentCollection from './collections/component-collection'
import ScreenCollection from './collections/screen-collection'
import IconCollection from './collections/icon-collection'

//===============> Private

const LoadComponent = (key) => {
  if (key in ComponentCollection) {
    return ComponentCollection[key]
  }

  console.error(`Element [${key}] does not exist in component collection!`)
  return null
}

const LoadScreen = (key) => {
  if (key in ScreenCollection) {
    return ScreenCollection[key]
  }

  console.error(`Element [${key}] does not exist in screen collection!`)
  return null
}

const LoadIcon = (key) => {
  if (key in IconCollection) {
    return IconCollection[key]
  }

  console.error(`Element [${key}] does not exist in icon collection!`)
  return null
}

//===============> Public

export const GetComponent = (key) => {
  const component = LoadComponent(key)

  if (component == null) return <></>

  const _lazy = lazy(component)

  return _lazy
}

export const GetScreen = (key) => {
  const screen = LoadScreen(key)

  if (screen == null) return <></>

  const _lazy = lazy(screen)

  return _lazy
}

export const GetIcon = (key, size = 'm') => {
  const icon = LoadIcon(key)

  if (icon == null) return <></>

  const _lazy = lazy(icon)

  return (
    <Suspense fallback={<></>}>
      <_lazy size={size} />
    </Suspense>
  )
}
