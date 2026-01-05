/* eslint-disable prettier/prettier */
import { useEffect, useState } from 'react'
import { getLogFiles, readLogFile } from '@src/app/infrastructure/process'
import { debounce } from 'lodash'

const useLogs = () => {
  const [initialLogFiles, setInitialLogFiles] = useState([])
  const [logFiles, setLogFiles] = useState([])
  const [logSearchText, setLogSearchText] = useState('')

  useEffect(() => {
    onReadLogFiles()
  }, [])

  useEffect(() => {
    setLogFiles(
      initialLogFiles.filter((x) =>
        x
          ?.toString()
          .toLowerCase()
          .includes(logSearchText?.toLowerCase() || '')
      )
    )
  }, [logSearchText])

  const onReadLogFiles = async () => {
    const files = await getLogFiles()
    setLogFiles(files)
    setInitialLogFiles(files)
  }

  const onFileSelect = async (file) => {
    return await readLogFile(file)
  }

  const handleSearchLogsDebounce = debounce(async (value) => {
    setLogSearchText(value)
  }, 500)

  const onLogsSearchChange = ({ target }) => {
    handleSearchLogsDebounce(target.value)
  }

  const refreshLogs = () => {
    onReadLogFiles()
  }

  return {
    logFiles,
    onLogsSearchChange,
    onFileSelect,
    refreshLogs
  }
}

export default useLogs
