/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import { useEffect, useRef, useState } from 'react'
import styled from '@emotion/styled'
import Highlighter from 'react-highlight-words'
import { useTranslation } from '@domain/administration/stores'
import Alert from '@ui/components/alert'
import Button from '@ui/components/button'
import Modal from '@ui/components/modal'
import TextInput from '../inputs/text-input'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2rem;
  overflow: hidden;
  height: 100%;

  & .actions {
    display: flex;
    gap: 1rem;
  }

  .highlight-text {
    background: #90caf9;
  }

  .active-highlight-text {
    background: yellow;
  }

  .text-content {
    white-space: pre-wrap;
  }

  & .content {
    overflow-y: scroll;
    border: 1px solid gray;
    border-radius: 0.25rem;
    padding: 0.5rem;
    height: 100%;
  }

  & .search {
    display: flex;
    gap: 1rem;
    margin-top: 0.5rem;
    width: 100%;
  }

  & .search-buttons {
    display: flex;
    gap: 0.5rem;
    align-items: center;
    flex-grow: 1;
  }

  & .nav-buttons {
    display: flex;
    gap: 0.5rem;
    align-items: center;
    flex-grow: 1;
  }
`
const ModalContent = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2rem;
`

const LogFile = ({ fileName, onSelect }) => {
  const contentRef = useRef()
  const [content, setContent] = useState('')
  const [search, setSearch] = useState('')
  const [searchItems, setSearchItems] = useState([])
  const [searchText, setSearchText] = useState([])
  const [cursor, setCursor] = useState(0)
  const modalRef = useRef(null)
  const { t } = useTranslation()

  const onViewFile = async () => {
    const content = await onSelect()
    setContent(content)
    modalRef.current?.showModal()
  }

  const onClose = () => {
    setContent('')
    modalRef.current?.close()
    setCursor(0)
    setSearchItems([])
    setSearch('')
  }

  const scrollToBottom = () => {
    contentRef?.current?.scrollTo({
      top: contentRef?.current?.scrollHeight,
      behavior: 'instant'
    })
  }

  useEffect(() => {
    var item = searchItems.at(cursor)
    item?.scrollIntoView({ behavior: 'instant', block: 'center' })
  }, [cursor])

  useEffect(() => {
    var highlights = document.getElementsByClassName('highlight-text')
    setSearchItems(Array.from(highlights))
    setCursor(0)
  }, [search])

  useEffect(() => {
    if (open) {
      setTimeout(() => {
        scrollToBottom()
      }, 5)
    }
  }, [open])

  return (
    <>
      <Alert
        variant="outlined"
        severity="info"
        text={fileName}
        onClick={onViewFile}
        buttonText={t('View')}
      />

      <Modal ref={modalRef} onClose={onClose}>
        <ModalContent>
          <h1>{fileName}</h1>

          <Container>
            <div className="search">
              <TextInput
                label={'Search'}
                value={searchText}
                onChange={(e) => {
                  setSearchText(e.target.value)
                }}
              />
              <div className="search-buttons">
                <Button
                  className="to-right"
                  onClick={() => {
                    setSearch(searchText)
                    scrollToBottom()
                  }}
                >
                  {t('Apply')}
                </Button>
                <Button
                  className="to-right"
                  onClick={() => {
                    setSearch('')
                    setSearchText('')
                    scrollToBottom()
                  }}
                >
                  {t('Clear')}
                </Button>
              </div>
            </div>
            <div className="nav-buttons">
              <Button className="to-right" onClick={scrollToBottom}>
                {t('To bottom')}
              </Button>

              <Button
                className="to-right"
                onClick={() => {
                  if (cursor > searchItems?.length - 2) setCursor(0)
                  else setCursor(cursor + 1)
                }}
              >
                {t('Next')}
              </Button>

              <Button
                className="to-right"
                onClick={() => {
                  if (cursor < 1) setCursor(searchItems?.length - 1)
                  else setCursor(cursor - 1)
                }}
              >
                {t('Previous')}
              </Button>
            </div>

            <div className="content" ref={contentRef}>
              <Highlighter
                className={'text-content'}
                activeIndex={cursor || 0}
                highlightClassName="highlight-text"
                searchWords={[search]}
                autoEscape={true}
                textToHighlight={content}
                activeClassName={'active-highlight-text'}
              />
            </div>
          </Container>
        </ModalContent>
      </Modal>
    </>
  )
}

export default LogFile
