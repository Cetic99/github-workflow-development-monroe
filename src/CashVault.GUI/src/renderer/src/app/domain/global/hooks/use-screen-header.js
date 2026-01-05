/* eslint-disable prettier/prettier */
import { useGlobalActions } from '@domain/global/stores'
import { useEffect } from 'react'

const useScreenHeader = (header) => {
  const { setScreenHeader } = useGlobalActions()

  useEffect(() => {
    setScreenHeader(header)
  }, [])
}

export default useScreenHeader
