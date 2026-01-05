/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { useRef, useState } from 'react'

const Container = styled.div`
  display: flex;
  flex-wrap: nowrap;
  gap: 2rem;
  border-bottom: 1.5px solid var(--primary-medium);
  overflow-x: auto;
  overflow-y: hidden;
  scrollbar-width: none; /* Firefox */
  -ms-overflow-style: none; /* IE/Edge */
  &::-webkit-scrollbar {
    display: none; /* Chrome/Safari */
  }

  cursor: grab;
  user-select: none;
`

const TabItem = styled.div`
  padding: 0 0 1rem 0;
  transition: all 0.2s ease-in;

  font-family: Poppins;
  font-size: 1.75rem;
  line-height: 1.875rem;
  color: var(--text-medium);
  border-bottom: 7px solid transparent;

  word-break: keep-all;
  white-space: nowrap;
  overflow-wrap: normal;

  ${(p) =>
    p.active &&
    `
      color: black;
      font-weight: 600;
      font-size: 1.75rem;
      line-height: 1.875rem;
      border-bottom: 7px solid var(--primary-medium);
    `}
`

export const Tab = ({ name, active = false, onSelect = () => {} }) => {
  return (
    <TabItem active={active} onClick={(e) => onSelect(e)}>
      {name}
    </TabItem>
  )
}

const Tabs = ({ className = '', children }) => {
  const containerRef = useRef(null)
  const [isDragging, setIsDragging] = useState(false)
  const [startX, setStartX] = useState(0)
  const [scrollLeft, setScrollLeft] = useState(0)
  const [dragMoved, setDragMoved] = useState(false)

  const handleMouseDown = (e) => {
    setIsDragging(true)
    setDragMoved(false)
    setStartX(e.pageX - containerRef.current.offsetLeft)
    setScrollLeft(containerRef.current.scrollLeft)
  }

  const handleMouseLeave = () => setIsDragging(false)
  const handleMouseUp = () => {
    setIsDragging(false)
    setTimeout(() => setDragMoved(false), 50)
  }

  const handleMouseMove = (e) => {
    if (!isDragging) return
    e.preventDefault()
    const x = e.pageX - containerRef.current.offsetLeft
    const walk = (x - startX) * 1.5
    if (Math.abs(x - startX) > 5) setDragMoved(true)
    containerRef.current.scrollLeft = scrollLeft - walk
  }

  //Prevents onClick event propagation when dragging.
  const handleClickCapture = (e) => {
    if (dragMoved) {
      e.stopPropagation()
      e.preventDefault()
    }
  }

  return (
    <Container
      className={className}
      ref={containerRef}
      onMouseDown={handleMouseDown}
      onMouseLeave={handleMouseLeave}
      onMouseUp={handleMouseUp}
      onMouseMove={handleMouseMove}
      onClickCapture={handleClickCapture}
      style={{ cursor: isDragging ? 'grabbing' : 'grab' }}
    >
      {children}
    </Container>
  )
}

export default Tabs
