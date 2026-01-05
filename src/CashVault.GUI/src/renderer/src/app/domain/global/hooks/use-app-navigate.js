import { useContext } from 'react'
import { NavigationContext } from '@ui/components/navigation-provider'

export const useAppNavigate = () => useContext(NavigationContext)
