import { createContext } from 'react'
import { useNavigate, useLocation } from 'react-router-dom'

export const NavigationContext = createContext()

export const NavigationProvider = ({ children }) => {
  const navigate = useNavigate()
  const location = useLocation()

  const delayedNavigate = (to, direction) => {
    const wave =
      document.querySelector('.bg-image-container') || document.querySelector('.wave-element')

    if (
      location.pathname != '/language' &&
      wave &&
      (window.innerHeight >= 832 || wave.classList.contains('wave-element'))
    ) {
      wave.classList.remove('wave-exit-up', 'wave-exit-down')
      wave.classList.add(direction === 'up' ? 'wave-exit-up' : 'wave-exit-down')

      wave.addEventListener('animationend', () => navigate(to), { once: true })
    } else {
      navigate(to)
    }
  }

  return <NavigationContext.Provider value={delayedNavigate}>{children}</NavigationContext.Provider>
}
