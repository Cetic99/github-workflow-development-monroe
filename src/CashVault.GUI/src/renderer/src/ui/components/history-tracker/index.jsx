/* eslint-disable prettier/prettier */
/* eslint-disable react/prop-types */
// HistoryTracker.jsx
import { createContext, useState, useEffect } from 'react'
import { useLocation, useNavigate } from 'react-router-dom'

export const HistoryContext = createContext()

const HistoryTracker = ({ children }) => {
  const [historyStack, setHistoryStack] = useState(['/'])
  const location = useLocation()
  const navigate = useNavigate()

  const getPrefix = (path) => {
    if (!path || path === '/') return '/'
    if (path.startsWith('/atm')) return '/atm'
    if (path.startsWith('/parcel-locker')) return '/parcel-locker'
    if (path.startsWith('/retail')) return '/retail'
    return '/'
  }

  useEffect(() => {
    if (location.pathname === '/') {
      setHistoryStack(['/'])
      return
    }

    setHistoryStack((prevStack) => {
      let copy = [...prevStack]
      if (copy.length === 0) copy = ['/']

      const lastItem = copy.at(-1)
      const lastPrefix = getPrefix(lastItem)
      const currentPrefix = getPrefix(location.pathname)

      if (lastPrefix !== currentPrefix && location.pathname !== '/') {
        return ['/', location.pathname]
      }

      if (lastItem === location.pathname) {
        return copy
      }

      copy.push(location.pathname)
      return copy
    })
  }, [location.pathname])

  const onGoBack = () => {
    if (historyStack.length > 1) {
      const newStack = [...historyStack]
      newStack.pop() // Remove current path

      const previousPath = newStack.at(-1)

      setHistoryStack(newStack)

      if (previousPath) {
        navigate(previousPath)
      } else {
        navigate('/')
      }
    }
  }

  return (
    <HistoryContext.Provider
      value={{ canGoBack: historyStack.length > 1, onGoBack, setHistoryStack }}
    >
      {children}
    </HistoryContext.Provider>
  )
}

export default HistoryTracker
