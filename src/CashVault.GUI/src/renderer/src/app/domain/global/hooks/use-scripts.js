/* eslint-disable prettier/prettier */
import { getScriptFiles, runScript } from '@src/app/infrastructure/process'
import { debounce } from 'lodash'
import { useEffect, useState } from 'react'

const useScripts = () => {
  const [initialScriptFiles, setInitialScriptFiles] = useState([])
  const [scriptFiles, setScriptFiles] = useState([])
  const [scriptSearchText, setScriptSearchText] = useState('')

  useEffect(() => {
    onReadScriptFiles()
  }, [])

  useEffect(() => {
    setScriptFiles(initialScriptFiles.filter((x) => x?.toLowerCase()?.includes(scriptSearchText?.toLowerCase())))
  }, [scriptSearchText])

  const onReadScriptFiles = async () => {
    const scripts = await getScriptFiles()
    setScriptFiles(scripts)
    setInitialScriptFiles(scripts)
  }

  const onScriptSelect = async (file) => {
    return await runScript(file)
  }

  const handleSearchScriptsDebounce = debounce(async (value) => {
    setScriptSearchText(value)
  }, 500)

  const onScriptsSearchChange = ({ target }) => {
    handleSearchScriptsDebounce(target.value)
  }

  const refreshScripts = () => {
    onReadScriptFiles()
  }

  return {
    scriptFiles,
    onScriptSelect,
    onScriptsSearchChange,
    refreshScripts
  }
}

export default useScripts
