/* eslint-disable react/prop-types */
import styled from '@emotion/styled'
import { useRef, useState } from 'react'

const Container = styled.div`
  display: flex;
  overflow-x: scroll;
  overflow-y: hidden;
  cursor: grab;
  scrollbar-width: none;
  -ms-overflow-style: none;

  &::-webkit-scrollbar {
    display: none;
  }

  &:active {
    cursor: grabbing;
  }
`

const ItemsCarousel = ({ children }) => {
  const carouselRef = useRef(null)
  const [isDown, setIsDown] = useState(false)
  const [startX, setStartX] = useState(0)
  const [scrollLeft, setScrollLeft] = useState(0)
  const [hasDragged, setHasDragged] = useState(false)

  const handleMouseDown = (e) => {
    setIsDown(true)
    setStartX(e.pageX - carouselRef.current.offsetLeft)
    setScrollLeft(carouselRef.current.scrollLeft)
    setHasDragged(false)
  }

  const handleMouseLeave = () => setIsDown(false)
  const handleMouseUp = () => {
    setIsDown(false)
    // Mali delay da se ne registrira click odmah nakon drag-a
    setTimeout(() => setHasDragged(false), 0)
  }

  const handleMouseMove = (e) => {
    if (!isDown) return
    e.preventDefault()
    const x = e.pageX - carouselRef.current.offsetLeft
    const walk = (x - startX) * 1.5

    if (Math.abs(walk) > 5) setHasDragged(true) // ako je povukao više od 5px, smatramo to dragom

    carouselRef.current.scrollLeft = scrollLeft - walk
  }

  // spriječi click evente unutar karusela ako je bio drag
  const handleClickCapture = (e) => {
    if (hasDragged) {
      e.stopPropagation()
      e.preventDefault()
    }
  }

  return (
    <Container
      ref={carouselRef}
      onMouseDown={handleMouseDown}
      onMouseLeave={handleMouseLeave}
      onMouseUp={handleMouseUp}
      onMouseMove={handleMouseMove}
      onClickCapture={handleClickCapture}
    >
      {children}
    </Container>
  )
}

export default ItemsCarousel
